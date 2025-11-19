using CoffeTime.Datos.Conexion;
using CoffeTime.Datos.Repositorios;
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
                var usuarios = await repo.GetAll();

                MessageBox.Show($"Conexión OK ✔\nUsuarios en la base: {usuarios.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Error:\n" + ex.Message);
            }
        }

    }
}
