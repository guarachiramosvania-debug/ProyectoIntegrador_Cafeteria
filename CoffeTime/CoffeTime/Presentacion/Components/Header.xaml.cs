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

        // 🔥 NAVEGACIÓN DINÁMICA ENTRE VENTANAS
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

            if (nuevaVentana != null)
            {
                nuevaVentana.Show();
                Window.GetWindow(this)?.Close(); // Cierra la ventana actual
            }
        }

        // 🔥 CERRAR SESIÓN
        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["NombreUsuario"] = null;
            App.Current.Properties["RolUsuario"] = null;

            new LoginView().Show();
            Window.GetWindow(this)?.Close();
        }
    }
}
