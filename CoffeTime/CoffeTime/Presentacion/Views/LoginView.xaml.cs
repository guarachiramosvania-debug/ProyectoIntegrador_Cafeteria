using CoffeTime.Negocio.Servicios;
using CoffeTime.Datos.Repositorios;
using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Limpiar cualquier mensaje de error anterior 
            ErrorTextBlock.Text = string.Empty;

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordPasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                // 2. Mostrar el error en el TextBlock 
                ErrorTextBlock.Text = "Debe ingresar usuario y contraseña.";
                return;
            }

            var repo = new UsuarioRepository();
            var service = new UsuarioService(repo);

            var usuario = await service.AutenticarAsync(username, password);

            if (usuario != null)
            {
                await repo.ActualizarUltimoLoginAsync(usuario.IdUsuario);
                await repo.ActualizarOnlineAsync(usuario.IdUsuario, true);
                App.Current.Properties["IdUsuario"] = (long)usuario.IdUsuario;

                
                App.Current.Properties["NombreUsuario"] = usuario.NombreUsuario;
                App.Current.Properties["RolUsuario"] = usuario.Rol;

                DashboardView dashboard = new DashboardView();
                Application.Current.MainWindow = dashboard;
                dashboard.Show();
                this.Close();
            }
            else
            {
                // 3. Mostrar el error de autenticación en el TextBlock
                ErrorTextBlock.Text = "Usuario o contraseña incorrectos.";
            }

        }
    }
}
