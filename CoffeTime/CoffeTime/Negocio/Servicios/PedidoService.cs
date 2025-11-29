using CoffeTime.Datos.Conexion;
using CoffeTime.Negocio.Modelos;
using CoffeTime.Negocio.Modelos.DTO;
using CoffeTime.Negocio.Models;
using Supabase;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CoffeTime.Negocio.Servicios
{
    public class PedidoService
    {
        private readonly Client _client;

        public PedidoService()
        {
            _client = SupabaseContext.Client;
        }

        // ====================================================
        // OBTENER LISTA LISTA DE PEDIDOS
        // ====================================================
        public async Task<List<PedidoDTO>> ObtenerPedidosAsync()
        {
            // 1?? Cargar TODO de golpe (solo 3 queries)
            var pedidosResp = await _client.From<Pedido>().Get();
            var detallesResp = await _client.From<DetallePedido>().Get();
            var productosResp = await _client.From<Producto>().Get();

            var pedidos = pedidosResp.Models;
            var detalles = detallesResp.Models;
            var productos = productosResp.Models.ToDictionary(p => p.Id);

            List<PedidoDTO> lista = new();

            foreach (var p in pedidos.OrderByDescending(x => x.IdPedido))
            {
                // Obtener detalles del pedido (filtrado en memoria = instantáneo)
                var dets = detalles.Where(d => d.IdPedido == p.IdPedido).ToList();

                // Construir lista de productos “2 x Café Latte”
                var productosTexto = new List<string>();

                foreach (var d in dets)
                {
                    if (productos.TryGetValue((int)d.IdProducto, out var prod))
                    {
                        productosTexto.Add($"{d.Cantidad} x {prod.Nombre}");
                    }

                }

                lista.Add(new PedidoDTO
                {
                    IdPedido = p.IdPedido,
                    NombrePedido = $"Pedido #{p.NumeroPedido}",
                    Fecha = p.Fecha,
                    Estado = p.Estado,
                    MetodoPago = p.MetodoPago,
                    Total = p.Total,
                    Productos = productosTexto
                });
            }

            return lista;
        }

        // ====================================================
        // PAGAR PEDIDO (NO REESCRIBE ESTADO CANCELADO)
        // ====================================================
        public async Task MarcarComoPagado(long idPedido)
        {
            var sw = new Stopwatch();
            sw.Start();

            Debug.WriteLine("=== INICIANDO PROCESO DE PAGAR PEDIDO ===");

            // 1?? Cambiar estado a Pagado
            Debug.WriteLine($"? Cambiando estado del pedido {idPedido}...");
            await _client
                .From<Pedido>()
                .Where(p => p.IdPedido == idPedido)
                .Set(x => x.Estado, "Pagado")
                .Update();

            Debug.WriteLine($"? Estado cambiado en {sw.ElapsedMilliseconds} ms");

            // 2?? Obtener detalles del pedido
            Debug.WriteLine("? Obteniendo detalles...");
            var detalles = await _client
                .From<DetallePedido>()
                .Where(d => d.IdPedido == idPedido)
                .Get();

            Debug.WriteLine($"? Detalles obtenidos en {sw.ElapsedMilliseconds} ms");

            // 3?? Por cada detalle, obtener insumos
            foreach (var det in detalles.Models)
            {
                Debug.WriteLine($"? Detalle: IdProducto={det.IdProducto}, Cantidad={det.Cantidad}");

                var piList = await _client
                    .From<ProductoInsumo>()
                    .Where(pi => pi.IdProducto == det.IdProducto)
                    .Get();

                Debug.WriteLine($"? ProductoInsumo obtenido en {sw.ElapsedMilliseconds} ms");

                foreach (var pi in piList.Models)
                {
                    Debug.WriteLine($"? Descontando insumo {pi.IdInsumo}");

                    var insumo = await _client
                        .From<Insumo>()
                        .Where(i => i.IdInsumo == pi.IdInsumo)
                        .Single();

                    Debug.WriteLine($"? Insumo obtenido en {sw.ElapsedMilliseconds} ms");

                    insumo.StockActual -= (pi.Cantidad * det.Cantidad);

                    await _client
                        .From<Insumo>()
                        .Where(i => i.IdInsumo == pi.IdInsumo)
                        .Update(insumo);

                    Debug.WriteLine($"? Insumo actualizado en {sw.ElapsedMilliseconds} ms");
                }
            }

            Debug.WriteLine($"=== PROCESO COMPLETO EN {sw.ElapsedMilliseconds} ms ===");
        }


        // ====================================================
        // CANCELAR PEDIDO
        // ====================================================
        public async Task CancelarPedidoAsync(long idPedido)
        {
            await _client
                .From<Pedido>()
                .Where(p => p.IdPedido == idPedido)
.Set(x => x.Estado, "Cancelado")
                .Update();
        }

        // ====================================================
        // CREAR PEDIDO
        // ====================================================
        public async Task<bool> CrearPedidoAsync(string metodoPago, long idUsuario, List<(int idProducto, int cantidad)> items)
        {
            try
            {
                if (items == null || items.Count == 0)
                    return false;

                // 1?? Cargar TODOS los productos una sola vez
                var productosResp = await _client.From<Producto>().Get();
                var productosDic = productosResp.Models.ToDictionary(p => p.Id);

                decimal total = 0;
                var detallesAInsertar = new List<DetallePedido>();

                // 2?? Construir los detalles en memoria y calcular el total
                foreach (var item in items)
                {
                    if (!productosDic.TryGetValue(item.idProducto, out var prod))
                        continue; // si no lo encuentra, lo salta

                    decimal subtotal = prod.Precio * item.cantidad;
                    total += subtotal;

                    detallesAInsertar.Add(new DetallePedido
                    {
                        IdProducto = item.idProducto,
                        Cantidad = item.cantidad,
                        Subtotal = subtotal
                    });
                }

                if (total <= 0 || detallesAInsertar.Count == 0)
                    return false;

                // 3?? Obtener siguiente número de pedido
                var response = await _client
                    .From<Pedido>()
                    .Order(p => p.IdPedido, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Limit(1)
                    .Get();

                long numero = 1;
                if (response.Models.Count > 0)
                    numero = response.Models[0].NumeroPedido + 1;

                // 4?? Crear pedido ya con Total correcto (sin UPDATE después)
                var pedido = new Pedido
                {
                    NumeroPedido = numero,
                    Fecha = DateTime.Now,
                    Estado = "Pendiente",      // cuidado con mayús/minús según tu CHECK
                    MetodoPago = metodoPago,
                    Total = total,
                    IdUsuario = idUsuario
                };

                // Insertar pedido
                var insertPedido = await _client
                    .From<Pedido>()
                    .Insert(pedido);

                var pedidoCreado = insertPedido.Models.FirstOrDefault();
                if (pedidoCreado == null)
                    return false;

                long idPedido = pedidoCreado.IdPedido;

                // 5?? Insertar detalles usando la lista preparada
                foreach (var det in detallesAInsertar)
                {
                    det.IdPedido = idPedido;
                    await _client.From<DetallePedido>().Insert(det);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear pedido: " + ex.Message);
            }
        }
        public async Task<bool> PagarPedidoRPC(long idPedido)
        {
            try
            {
                var resp = await _client.Rpc("pagar_pedido", new { p_id = idPedido });
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR RPC: " + ex.Message);
                return false;
            }
        }



    }
}
