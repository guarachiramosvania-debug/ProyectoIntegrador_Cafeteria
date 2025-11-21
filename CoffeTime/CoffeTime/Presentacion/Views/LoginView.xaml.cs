using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CoffeTime.Presentacion.Views
{
    /// <summary>
    /// Lógica de interacción para LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            // El try-catch envuelve la inicialización para atrapar errores de carga de componentes.
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico al inicializar la ventana de Login: {ex.Message}", "Error de Carga", MessageBoxButton.OK, MessageBoxImage.Error);
                // Si la carga falla, la aplicación no puede continuar
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Manejador de evento click para el botón "Iniciar Sesión".
        /// Contiene las validaciones y el bloque try-catch.
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordPasswordBox.Password;

            // --- 1. VALIDACIÓN DE CAMPOS ---
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Por favor, ingrese su nombre de usuario.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                UsernameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor, ingrese su contraseña.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordPasswordBox.Focus();
                return;
            }

            // --- 2. LÓGICA DE AUTENTICACIÓN Y TRY-CATCH ---
            try
            {
                bool isAuthenticated = AuthenticateUser(username, password);

                if (isAuthenticated)
                {
                    MessageBox.Show("¡Inicio de sesión exitoso!", "Bienvenido", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Abrir la ventana principal (Dashboard)
                    DashboardView dashboard = new DashboardView();
                    dashboard.Show();

                    // Cerrar la ventana de login
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos. Intente de nuevo.", "Error de Autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Net.WebException ex)
            {
                // Captura errores específicos de red (si la autenticación es vía API)
                MessageBox.Show($"Error de conexión al servidor: {ex.Message}", "Error de Red", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Captura cualquier otro error inesperado (ej. base de datos, servicio)
                MessageBox.Show($"Ocurrió un error inesperado durante el login: {ex.Message}", "Error del Sistema", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Simula el proceso de verificación de credenciales.
        /// En un proyecto real, esta lógica interactuaría con una capa de negocio o base de datos.
        /// </summary>
        /// <param name="user">Nombre de usuario</param>
        /// <param name="pass">Contraseña</param>
        /// <returns>True si las credenciales son válidas, False en caso contrario.</returns>
        private bool AuthenticateUser(string user, string pass)
        {
            // Credenciales de prueba
            const string TEST_USER = "admin";
            const string TEST_PASS = "12345";

            // Simulación de un proceso de autenticación lento o complejo
            // System.Threading.Thread.Sleep(500); 

            if (user == TEST_USER && pass == TEST_PASS)
            {
                return true;
            }

            // Simulación de un error específico para demostrar el try-catch
            if (user == "error")
            {
                throw new Exception("Error simulado de base de datos.");
            }

            return false;
        }
    }
}
