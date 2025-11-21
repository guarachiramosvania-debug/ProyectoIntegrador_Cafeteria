using CoffeTime.Datos.Conexion;
using CoffeTime.Datos.Repositorios;
using CoffeTime.Presentacion.Views;
using System.Windows;

namespace CoffeTime
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ProbarSupabase_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void BtnProveedores_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new ProveedoresView();
            ventana.Show();
        }

        private void btnUsuarios(object sender, RoutedEventArgs e)
        {
            var ventana = new UsuariosView();
            ventana.Show();
        }

        private void btndashboard(object sender, RoutedEventArgs e)
        {
            DashboardView dash = new DashboardView();

            Application.Current.MainWindow = dash;

            dash.Show();
            this.Close();
        }

    }
}
