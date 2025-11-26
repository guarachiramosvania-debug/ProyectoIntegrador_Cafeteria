using CoffeTime.Negocio.Servicios;
using CoffeTime.Datos.Repositorios;
using System;
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
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordPasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Debe ingresar usuario y contraseña.");
                return;
            }

            var repo = new UsuarioRepository();
            var service = new UsuarioService(repo);

            var usuario = await service.AutenticarAsync(username, password);

            if (usuario != null)
            {
                // Guardamos datos en App
                App.Current.Properties["NombreUsuario"] = usuario.NombreUsuario;
                App.Current.Properties["RolUsuario"] = usuario.Rol;
                App.Current.Properties["IdUsuario"] = usuario.IdUsuario;

                // Marcar ONLINE + ultimo login
                usuario.Online = true;
                usuario.UltimoLogin = DateTime.Now;
                await repo.ActualizarUsuarioAsync(usuario);

                var dashboard = new DashboardView();
                Application.Current.MainWindow = dashboard;
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.");
            }
        }

    }
}
