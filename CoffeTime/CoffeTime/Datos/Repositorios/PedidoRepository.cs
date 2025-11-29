using CoffeTime.Datos.Conexion;
using CoffeTime.Negocio.Models;   // Pedido, PedidoVistaDto
using CoffeTime.Negocio.Modelos; // DetallePedido, Producto
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeTime.Datos.Repositorios
{
    public class PedidoRepository
    {
        private readonly Client _db;

        public PedidoRepository()
        {
            _db = SupabaseContext.Client;
        }

        // =====================================================
        // 1) Obtener lista de pedidos con productos
        // =====================================================
        public async Task<List<PedidoVistaDto>> ObtenerPedidosAsync()
        {
            try
            {
                var resp = await _db
                    .From<Pedido>()
                    .Get();

                var pedidos = resp.Models
                    .OrderByDescending(p => p.IdPedido)
                    .ToList();

                var detalleRepo = new DetallePedidoRepository();
                var prodRepo = new ProductoRepository();

                var lista = new List<PedidoVistaDto>();

                foreach (var p in pedidos)
                {
                    var dto = new PedidoVistaDto
                    {
                        IdPedido = p.IdPedido,
                        NumeroPedido = p.NumeroPedido,
                        NombrePedido = $"Pedido #{p.NumeroPedido}",
                        Estado = p.Estado,
                        FechaHora = p.Fecha.ToString("dd/MM/yyyy HH:mm"),
                        MetodoPago = p.MetodoPago,
                        Total = p.Total.ToString("0.00"),
                        Productos = new List<string>()
                    };

                    // Detalles -> nombres de producto
                    var detalles = await detalleRepo.ObtenerPorPedido(p.IdPedido);

                    foreach (var det in detalles)
                    {
                        var prod = await prodRepo.GetById((int)det.IdProducto);
                        if (prod != null)
                        {
                            dto.Productos.Add($"{det.Cantidad} x {prod.Nombre}");
                        }
                    }

                    lista.Add(dto);
                }

                return lista;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR obteniendo pedidos: " + ex);
                return new List<PedidoVistaDto>();
            }
        }

        // =====================================================
        // 2) Cancelar pedido
        // =====================================================
        public async Task<bool> CancelarPedidoAsync(long idPedido)
        {
            try
            {
                var resp = await _db
                    .From<Pedido>()
                    .Where(p => p.IdPedido == idPedido)
                    .Get();

                var pedido = resp.Models.FirstOrDefault();
                if (pedido == null)
                    return false;

                pedido.Estado = "Cancelado";

                await _db.From<Pedido>().Update(pedido);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR cancelando pedido: " + ex);
                return false;
            }
        }

        // =====================================================
        // 3) Crear nuevo pedido + detalle
        // =====================================================
        public async Task<bool> CrearPedidoAsync(
            string metodoPago,
            long idUsuario,
            List<(int idProducto, int cantidad)> items)
        {
            if (items == null || items.Count == 0)
                return false;

            var prodRepo = new ProductoRepository();

            // --- Calcular total ---
            decimal total = 0m;
            var preciosPorProducto = new Dictionary<int, decimal>();

            foreach (var it in items)
            {
                var prod = await prodRepo.GetById(it.idProducto);
                if (prod == null) continue;

                preciosPorProducto[it.idProducto] = prod.Precio;
                total += prod.Precio * it.cantidad;
            }

            if (total <= 0)
                return false;

            // --- Numero de pedido (último + 1) ---
            long numeroPedido = 1;
            var ultResp = await _db
                .From<Pedido>()
                .Order(p => p.NumeroPedido, Supabase.Postgrest.Constants.Ordering.Descending)
                .Limit(1)
                .Get();

            var ultimo = ultResp.Models.FirstOrDefault();
            if (ultimo != null)
                numeroPedido = ultimo.NumeroPedido + 1;

            // --- Insertar pedido ---
            var nuevo = new Pedido
            {
                NumeroPedido = numeroPedido,
                Fecha = DateTime.Now,
                Estado = "Pendiente",
                MetodoPago = metodoPago,
                Total = total,
                IdUsuario = idUsuario
            };

            var insResp = await _db.From<Pedido>().Insert(nuevo);
            var pedidoInsertado = insResp.Models.FirstOrDefault();
            if (pedidoInsertado == null)
                return false;

            long idPedido = pedidoInsertado.IdPedido;

            // --- Insertar detalle ---
            foreach (var it in items)
            {
                if (!preciosPorProducto.TryGetValue(it.idProducto, out var precioUnit))
                    continue;

                var det = new DetallePedido
                {
                    IdPedido = (int)idPedido,
                    IdProducto = it.idProducto,
                    Cantidad = it.cantidad,
                    Subtotal = precioUnit * it.cantidad
                };

                await _db.From<DetallePedido>().Insert(det);
            }

            return true;
        }
    }
}
