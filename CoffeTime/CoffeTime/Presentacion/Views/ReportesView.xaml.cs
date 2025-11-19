using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CoffeTime.Negocio.Servicios;

namespace CoffeTime.Presentacion.Views
{
    public partial class ReportesView : Window
    {
        private readonly ReporteService _reporteService;

        public ReportesView()
        {
            InitializeComponent();

            _reporteService = new ReporteService();

            Loaded += ReportesView_Loaded;
        }

        private async void ReportesView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarResumenGeneralAsync();
            await CargarVentasPorDiaAsync();
            await CargarProductosMasVendidosAsync();
        }

        private async Task CargarResumenGeneralAsync()
        {
            try
            {
                var resumen = await _reporteService.ObtenerResumenGeneralAsync();

                txtVentasTotales.Text = resumen.VentasTotales.ToString("C2");
                txtTotalPedidos.Text = resumen.TotalPedidos.ToString();
                txtTicketPromedio.Text = resumen.TicketPromedio.ToString("C2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar resumen general: " + ex.Message);
            }
        }

        private async Task CargarVentasPorDiaAsync()
        {
            try
            {
                var lista = await _reporteService.ObtenerVentasPorDiaAsync();
                dgVentasPorDia.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ventas por día: " + ex.Message);
            }
        }

        private async Task CargarProductosMasVendidosAsync()
        {
            try
            {
                var lista = await _reporteService.ObtenerProductosMasVendidosAsync(DateTime.MinValue, DateTime.MaxValue);

                // agregar posición 1..n para mostrar numeración
                var conPosicion = lista
                    .Select((x, index) => new
                    {
                        Posicion = index + 1,
                        x.NombreProducto,
                        x.CantidadVendida,
                        x.MontoTotal
                    }).ToList();

                dgProductosMasVendidos.ItemsSource = conPosicion;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos más vendidos: " + ex.Message);
            }
        }

        private async void btnCargarVentasMensuales_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtAnioReportes.Text, out int anio))
                {
                    MessageBox.Show("Ingrese un año válido.");
                    return;
                }

                var lista = await _reporteService.ObtenerReporteMensualAsync(anio);

                var conNombreMes = lista.Select(x => new
                {
                    MesNombre = new DateTime(anio, x.Mes, 1).ToString("MMMM yyyy"),
                    x.CantidadPedidos,
                    x.TotalVentas
                }).ToList();

                dgVentasMensuales.ItemsSource = conNombreMes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ventas mensuales: " + ex.Message);
            }
        }

        private void btnExportarReporte_Click(object sender, RoutedEventArgs e)
        {
            // TODO: exportar a PDF/Excel si lo agregan al proyecto
            MessageBox.Show("Función de exportar pendiente de implementar.");
        }
    }
}
