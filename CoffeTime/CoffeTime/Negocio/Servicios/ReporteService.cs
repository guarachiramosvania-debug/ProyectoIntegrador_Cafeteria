using CoffeTime.Datos.Conexion;
using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    public class ReporteService
    {
        private readonly PedidoRepository _pedidoRepo;
        private readonly ProductoRepository _productoRepo;

        public ReporteService()
        {
            _pedidoRepo = new PedidoRepository();   // ? corregido
            _productoRepo = new ProductoRepository(); // ? corregido
        }

        // --------------------------------------------------------------------
        // RESUMEN GENERAL
        // --------------------------------------------------------------------
        public async Task<(decimal TotalVentas, int TotalPedidos, decimal TicketPromedio)>
            ObtenerResumenGeneralAsync()
        {
            var pedidos = await _pedidoRepo.GetAll();

            if (pedidos.Count == 0)
                return (0, 0, 0);

            decimal totalVentas = pedidos.Sum(p => p.Total);
            int totalPedidos = pedidos.Count;
            decimal ticketPromedio = totalVentas / totalPedidos;

            return (totalVentas, totalPedidos, ticketPromedio);
        }

        // --------------------------------------------------------------------
        // VENTAS POR DÍA
        // --------------------------------------------------------------------
        public async Task<Dictionary<DateTime, decimal>> ObtenerVentasPorDiaAsync()
        {
            var pedidos = await _pedidoRepo.GetAll();

            return pedidos
                .GroupBy(p => p.Fecha.Date)
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Total));
        }

        // --------------------------------------------------------------------
        // VENTAS MENSUALES
        // --------------------------------------------------------------------
        public async Task<Dictionary<string, decimal>> ObtenerVentasMensualesAsync()
        {
            var pedidos = await _pedidoRepo.GetAll();

            return pedidos
                .GroupBy(p => $"{p.Fecha:MMMM yyyy}")
                .ToDictionary(g => g.Key, g => g.Sum(p => p.Total));
        }

        // --------------------------------------------------------------------
        // PRODUCTOS MÁS VENDIDOS
        // --------------------------------------------------------------------
        public async Task<List<Producto>> ObtenerProductosMasVendidosAsync()
        {
            var productos = await _productoRepo.GetAll();

            // Temporal hasta que uses detalle_pedido
            return productos
                .OrderByDescending(p => p.Precio)
                .Take(5)
                .ToList();
        }
    }
}
