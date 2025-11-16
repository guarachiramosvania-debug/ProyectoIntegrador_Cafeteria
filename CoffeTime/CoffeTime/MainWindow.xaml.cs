using ProyectoIntegrador_Cafeteria.Datos.Conexion;
using ProyectoIntegrador_Cafeteria.Datos.Repositorios;
using System.Windows;

async void ProbarSupabase_Click(object sender, RoutedEventArgs e)
{
    try
    {
        // Inicializar conexión
        await SupabaseContext.InitializeAsync();

        // Probar SELECT de la tabla usuarios
        var repo = new UsuarioRepository();
        var usuarios = await repo.GetAll();

        MessageBox.Show($"Conexión OK ✔\nUsuarios en la base: {usuarios.Count}");
    }
    catch (Exception ex)
    {
        MessageBox.Show("❌ Error de conexión:\n" + ex.Message);
    }
}
