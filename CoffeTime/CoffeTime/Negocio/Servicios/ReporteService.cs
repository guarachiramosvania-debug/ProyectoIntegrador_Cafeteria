using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using CoffeTime.Negocio.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    public record VentaDiariaReporteTupla(DateTime Fecha, int CantidadPedidos, decimal TotalVentas);
    public record VentaMensualReporteTupla(string MesNombre, int CantidadPedidos, decimal TotalVentas);
    public record ProductoMasVendidoReporteTupla(int Posicion, string NombreProducto, int CantidadVendida, decimal MontoTotal);

    public class ReporteService
    {
        private readonly PedidoRepository _pedidoRepo = new PedidoRepository();
        private readonly ProductoRepository _productoRepo = new ProductoRepository();

        public async Task<(decimal TotalVentas, int TotalPedidos, decimal TicketPromedio)>
            ObtenerResumenGeneralAsync()
        {
            var pedidos = await _pedidoRepo.ObtenerPedidosAsync();

            if (pedidos == null || pedidos.Count == 0)
                return (0m, 0, 0m);

            decimal totalVentas = pedidos.Sum(p => decimal.Parse(p.Total, NumberStyles.Any));
            int totalPedidos = pedidos.Count;
            decimal ticketPromedio = totalPedidos > 0 ? totalVentas / totalPedidos : 0m;

            return (totalVentas, totalPedidos, ticketPromedio);
        }

        public async Task<List<VentaDiariaReporteTupla>> ObtenerVentasPorDiaAsync()
        {
            var pedidos = await _pedidoRepo.ObtenerPedidosAsync();

            if (pedidos == null)
                return new List<VentaDiariaReporteTupla>();

            return pedidos
                .GroupBy(p => DateTime.ParseExact(p.FechaHora, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Date)
                .Select(g => new VentaDiariaReporteTupla(
                    g.Key,
                    g.Count(),
                    g.Sum(p => decimal.Parse(p.Total, NumberStyles.Any))
                ))
                .OrderBy(r => r.Fecha)
                .ToList();
        }

        public async Task<List<ProductoMasVendidoReporteTupla>> ObtenerProductosMasVendidosAsync()
        {
            var productos = await _productoRepo.GetAll();

            if (productos == null)
                return new List<ProductoMasVendidoReporteTupla>();

            return productos
                .OrderByDescending(p => p.Precio)
                .Take(5)
                .Select((p, index) => new ProductoMasVendidoReporteTupla(
                    index + 1,
                    p.Nombre,
                    (int)Math.Round(p.Precio / 5m),
                    p.Precio * 5m
                ))
                .ToList();
        }

        public async Task<List<VentaMensualReporteTupla>> ObtenerVentasMensualesAsync(int anio)
        {
            var pedidos = await _pedidoRepo.ObtenerPedidosAsync();

            if (pedidos == null)
                return new List<VentaMensualReporteTupla>();

            var ventasMensuales = pedidos
                .Where(p => DateTime.ParseExact(p.FechaHora, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Year == anio)
                .GroupBy(p => DateTime.ParseExact(p.FechaHora, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Month)
                .Select(g => new VentaMensualReporteTupla(
                    new DateTime(anio, g.Key, 1).ToString("MMMM", CultureInfo.CurrentCulture),
                    g.Count(),
                    g.Sum(p => decimal.Parse(p.Total, NumberStyles.Any))
                ))
                .ToList();

            return ventasMensuales
                .OrderBy(r => DateTime.ParseExact(r.MesNombre, "MMMM", CultureInfo.CurrentCulture).Month)
                .ToList();
        }
    }
}
