using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CoffeTime.Presentacion.Views;

namespace CoffeTime.Presentacion.Components
{
    public partial class Header : UserControl
    {
        private readonly DispatcherTimer _timer;

        public Header()
        {
            InitializeComponent();

            // Mostrar nombre
            if (App.Current.Properties["NombreUsuario"] != null)
                TxtNombre.Text = App.Current.Properties["NombreUsuario"].ToString();

            // Mostrar rol
            if (App.Current.Properties["RolUsuario"] != null)
                TxtRol.Text = App.Current.Properties["RolUsuario"].ToString();

            // Reloj en tiempo real
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                TxtHora.Text = DateTime.Now.ToString("HH:mm:ss");
            };
            _timer.Start();
        }

        // Navegación entre pantallas
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
                // Si la ventana marcó que no tiene permisos, NO continuar
                if (nuevaVentana.Tag?.ToString() == "DENIED")
                {
                    nuevaVentana = null;  // evitar Show()
                    return;               // no cerrar la ventana actual
                }

                nuevaVentana.Show();
                Window.GetWindow(this)?.Close();
            }

        }

        // Logout
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
