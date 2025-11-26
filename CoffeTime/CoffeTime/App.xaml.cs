public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Application.Current.MainWindow.Closing += MainWindow_Closing;
    }
    private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        await CerrarSesionAutomatica();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await CerrarSesionAutomatica();
        base.OnExit(e);
    }

    private async Task CerrarSesionAutomatica()
    {
        try
        {
            if (App.Current.Properties["IdUsuario"] == null)
                return;

            long id = (long)App.Current.Properties["IdUsuario"];

            var repo = new UsuarioRepository();

            await repo.ActualizarOnlineAsync(id, false);
        }
        catch
        {
            // ignorar errores
        }
    }

       
    
}
