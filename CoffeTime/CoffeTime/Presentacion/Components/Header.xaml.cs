using System.Windows;
using System.Windows.Controls;
using CoffeTime.Presentacion.Views;

namespace CoffeTime.Presentacion.Components
{
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();

            if (App.Current.Properties["NombreUsuario"] != null)
                TxtNombre.Text = App.Current.Properties["NombreUsuario"].ToString();

            if (App.Current.Properties["RolUsuario"] != null)
                TxtRol.Text = App.Current.Properties["RolUsuario"].ToString();
        }
        //esto es un comentario
        // NAVEGACIÓN SIN CERRAR SESIÓN
        private void MenuClick(object sender, RoutedEventArgs e)
        {
            string destino = (sender as Button)?.Tag?.ToString();
            Window nuevaVentana = null;

            switch (destino)
            {
                case "Dashboard": nuevaVentana = new DashboardView(); break;
                case "Usuarios": nuevaVentana = new UsuariosView(); break;
                case "Productos": nuevaVentana = new ProductosView(); break;
                case "Pedidos": nuevaVentana = new PedidosView(); break;
                case "Inventario": nuevaVentana = new InventarioView(); break;
                case "Proveedores": nuevaVentana = new ProveedoresView(); break;
                case "Reportes": nuevaVentana = new ReportesView(); break;
            }

            nuevaVentana?.Show();
            Window.GetWindow(this)?.Close();
        }

        // BOTÓN SALIR (LOGOUT REAL)
        private async void LogoutClick(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["IdUsuario"] != null)
            {
                var repo = new UsuarioRepository();
                long id = (long)App.Current.Properties["IdUsuario"];
                await repo.ActualizarOnlineSoloAsync(id, false);
            }

            Application.Current.Shutdown();
        }
    }
}
