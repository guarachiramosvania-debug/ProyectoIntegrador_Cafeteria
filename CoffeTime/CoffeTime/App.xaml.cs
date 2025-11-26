public partial class App : Application
{
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
            var user = await repo.ObtenerPorIdAsync(id);

            if (user != null)
            {
                user.Online = false;
                user.UltimoLogin = DateTime.Now;
                await repo.ActualizarUsuarioAsync(user);
            }
        }
        catch
        {
            // ignoramos errores aquí para no romper el cierre de la app
        }
    }
}
