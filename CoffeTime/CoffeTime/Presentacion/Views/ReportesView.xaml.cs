using CoffeTime.Negocio.Servicios;
using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class ReportesView : Window
    {
        private readonly ReporteService _service = new ReporteService();

        public ReportesView()
        {
            InitializeComponent();
            CargarDatos();
        }

        private async void CargarDatos()
        {
            var resumen = await _service.ObtenerResumenGeneralAsync();

            txtVentasTotales.Text = $"${resumen.TotalVentas}";
            txtTotalPedidos.Text = resumen.TotalPedidos.ToString();
            txtTicketPromedio.Text = $"${resumen.TicketPromedio}";
        }
    }
}
