using CoffeTime.Negocio.Servicios;
using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class ReportesView : Window
    {
        private readonly ReporteService _service = new ReporteService();
        private readonly UsuarioRepository usuarioRepo = new UsuarioRepository();

        public ReportesView()
        {
            InitializeComponent();
            CargarDatos();
            MantenerUsuarioOnline();
        }
        private async void MantenerUsuarioOnline()
        {
            if (App.Current.Properties["IdUsuario"] is long id)
            {
                var usuario = await usuarioRepo.ObtenerPorIdAsync(id);

                if (usuario != null)
                {
                    usuario.Online = true;
                    await usuarioRepo.ActualizarOnlineAsync(usuario.IdUsuario, true);
                }
            }
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
