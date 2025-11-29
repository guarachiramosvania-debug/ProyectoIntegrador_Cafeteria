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
            await _client
                .From<Pedido>()
                .Where(p => p.IdPedido == idPedido)
                .Set(x => x.Estado, "Pagado")
                .Update();

            // Descontar stock
            var detalles = await _client
                .From<DetallePedido>()
                .Where(d => d.IdPedido == idPedido)
                .Get();
            foreach (var det in detalles.Models)
            {
                var piList = await _client
                    .From<ProductoInsumo>()
                    .Where(pi => pi.IdProducto == det.IdProducto)
                    .Get();

                foreach (var pi in piList.Models)
                {
                    var insumo = await _client
                        .From<Insumo>()
                        .Where(i => i.IdInsumo == pi.IdInsumo)
                        .Single();

                    insumo.StockActual -= (pi.Cantidad * det.Cantidad);

                    await _client
                        .From<Insumo>()
                        .Where(i => i.IdInsumo == pi.IdInsumo)
                        .Update(insumo);
                }
            }

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
                // Obtener siguiente número de pedido
                var response = await _client
                    .From<Pedido>()
                    .Order(p => p.IdPedido, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Limit(1)
                    .Get();

                long numero = 1;
                if (response.Models.Count > 0)
                    numero = response.Models[0].NumeroPedido + 1;

                // Crear pedido
                var pedido = new Pedido
                {
                    NumeroPedido = numero,
                    Fecha = DateTime.Now,
                    Estado = "Pendiente",
                    MetodoPago = metodoPago,
                    Total = 0,                     // Se actualizará después
                    IdUsuario = idUsuario
                };

                // INSERT pedido
                var insertPedido = await _client
                    .From<Pedido>()
                    .Insert(pedido);

                var pedidoCreado = insertPedido.Models.First();
                long idPedido = pedidoCreado.IdPedido;

                decimal total = 0;

                // Insertar detalles
                foreach (var item in items)
                {
                    var producto = await _client
                        .From<Producto>()
                        .Where(p => p.Id == item.idProducto)
                        .Single();

                    decimal subtotal = producto.Precio * item.cantidad;
                    total += subtotal;

                    var det = new DetallePedido
                    {
                        IdPedido = idPedido,
                        IdProducto = item.idProducto,
                        Cantidad = item.cantidad,
                        Subtotal = subtotal
                    };

                    await _client.From<DetallePedido>().Insert(det);
                }

                // Actualizar total real
                pedidoCreado.Total = total;

                await _client
                    .From<Pedido>()
                    .Where(p => p.IdPedido == idPedido)
                    .Update(pedidoCreado);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear pedido: " + ex.Message);
            }
        }

    }
}
