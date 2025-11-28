using CoffeTime.Datos.Conexion;
using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos; // Aún se necesita para el tipo 'Producto' y otros si existen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    // Define una tupla para facilitar el reporte de ventas por día
    public record VentaDiariaReporteTupla(DateTime Fecha, int CantidadPedidos, decimal TotalVentas);

    // Define una tupla para facilitar el reporte de ventas mensuales
    public record VentaMensualReporteTupla(string MesNombre, int CantidadPedidos, decimal TotalVentas);

    // Define una tupla para facilitar el reporte de productos
    public record ProductoMasVendidoReporteTupla(int Posicion, string NombreProducto, int CantidadVendida, decimal MontoTotal);


    public class ReporteService
    {
        private readonly PedidoRepository _pedidoRepo;
        private readonly ProductoRepository _productoRepo;

        public ReporteService()
        {
            // Asume que las Repositories están disponibles
            _pedidoRepo = new PedidoRepository();
            _productoRepo = new ProductoRepository();
        }

        // --------------------------------------------------------------------
        // RESUMEN GENERAL (Tupla de 3 valores)
        // --------------------------------------------------------------------
        public async Task<(decimal TotalVentas, int TotalPedidos, decimal TicketPromedio)>
            ObtenerResumenGeneralAsync()
        {
            var pedidos = await _pedidoRepo.GetAll();

            if (pedidos == null || pedidos.Count == 0)
                return (0m, 0, 0m);

            decimal totalVentas = pedidos.Sum(p => p.Total);
            int totalPedidos = pedidos.Count;
            decimal ticketPromedio = totalPedidos > 0 ? totalVentas / totalPedidos : 0m;

            return (totalVentas, totalPedidos, ticketPromedio);
        }

        // --------------------------------------------------------------------
        // VENTAS POR DÍA (Lista de tuplas estructuradas)
        // --------------------------------------------------------------------
        public async Task<List<VentaDiariaReporteTupla>> ObtenerVentasPorDiaAsync()
        {
            var pedidos = await _pedidoRepo.GetAll();

            if (pedidos == null) return new List<VentaDiariaReporteTupla>();

            return pedidos
                .GroupBy(p => p.Fecha.Date)
                .Select(g => new VentaDiariaReporteTupla(
                    g.Key,
                    g.Count(),
                    g.Sum(p => p.Total)
                ))
                .OrderBy(r => r.Fecha)
                .ToList();
        }

        // --------------------------------------------------------------------
        // PRODUCTOS MÁS VENDIDOS (Lista de tuplas estructuradas)
        // --------------------------------------------------------------------
        public async Task<List<ProductoMasVendidoReporteTupla>> ObtenerProductosMasVendidosAsync()
        {
            var productos = await _productoRepo.GetAll();

            if (productos == null) return new List<ProductoMasVendidoReporteTupla>();

            // Lógica Temporal Simplificada (simulando cantidad vendida)
            var reportes = productos
                .OrderByDescending(p => p.Precio)
                .Take(5)
                .Select((p, index) => new ProductoMasVendidoReporteTupla(
                    index + 1,
                    p.Nombre,
                    (int)Math.Round(p.Precio / 5), // Simulación de Cantidad
                    p.Precio * 5 // Simulación de Monto
                ))
                .ToList();

            return reportes;
        }

        
        public async Task<List<VentaMensualReporteTupla>> ObtenerVentasMensualesAsync(int anio)
        {
            var pedidos = await _pedidoRepo.GetAll();

            if (pedidos == null) return new List<VentaMensualReporteTupla>();

            var ventasMensuales = pedidos
                .Where(p => p.Fecha.Year == anio)
                .GroupBy(p => new { p.Fecha.Year, p.Fecha.Month })
                .Select(g => new VentaMensualReporteTupla(
                    new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM", System.Globalization.CultureInfo.CurrentCulture),
                    g.Count(),
                    g.Sum(p => p.Total)
                ))
                .ToList();

            // Asegurar que el orden sea cronológico por el mes
            return ventasMensuales
                .OrderBy(r => DateTime.ParseExact(r.MesNombre, "MMMM", System.Globalization.CultureInfo.CurrentCulture).Month)
                .ToList();
        }
    }
}