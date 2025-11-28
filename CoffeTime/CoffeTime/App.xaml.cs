using CoffeTime.Datos.Repositorios;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CoffeTime
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // ❌ Ya no se usa MainWindow.Closing aquí porque MainWindow aún no existe.
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await CerrarSesionAutomatica();
            base.OnExit(e);
        }

        /// <summary>
        /// Cerrar sesión automáticamente.
        /// Ejecutado desde: Logout, cierre normal, cierre de Dashboard.
        /// </summary>
        public async Task CerrarSesionAutomatica()
        {
            try
            {
                if (App.Current.Properties["IdUsuario"] == null)
                    return;

                long id = (long)App.Current.Properties["IdUsuario"];

                var repo = new UsuarioRepository();
                await repo.ActualizarOnlineSoloAsync(id, false);

            }
            catch { }
        }

    }
}
