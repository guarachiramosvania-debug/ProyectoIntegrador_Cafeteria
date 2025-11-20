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
            try
            {
                var repo = new UsuarioRepository();
                var usuarios = repo.GetAll();

            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error:\n" + ex.Message);
            }
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
    }
}
