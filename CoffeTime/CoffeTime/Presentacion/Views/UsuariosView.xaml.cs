using System;
using System.Linq;
using System.Windows;
using CoffeTime.Negocio.Servicios;
using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;

namespace CoffeTime.Presentacion.Views
{
    public partial class UsuariosView : Window
    {
        private readonly UsuarioService _service;
        private long? _idSeleccionado = null;

        public UsuariosView()
        {
            InitializeComponent();
            _service = new UsuarioService(new UsuarioRepository());

            Loaded += UsuariosView_Loaded;
        }

        // ============================
        // CARGAR LISTA DE USUARIOS
        // ============================
        private async void UsuariosView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var repo = new UsuarioRepository();
                var service = new UsuarioService(repo);

                var usuarios = await service.ObtenerTodosAsync();

                var lista = usuarios.Select(u => new
                {
                    IdUsuario = u.IdUsuario,
                    u.NombreUsuario,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Rol = u.Rol,

                    // ONLINE (campo nullable)
                    Estado = (u.Online ?? false) ? "🟢 Online" : "⚫ Offline",

                    // ULTIMO LOGIN (campo DateTime? nullable)
                    UltimoLogin = u.UltimoLogin == null
          ? "—"
          : u.UltimoLogin.Value.ToString("dd/MM/yyyy HH:mm")

                }).ToList();

                DataGridUsuarios.ItemsSource = lista;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message);
            }
        }



        // ============================
        // NUEVO USUARIO
        // ============================
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            UsuarioFormulario form = new UsuarioFormulario(null);
            form.ShowDialog();
            UsuariosView_Loaded(null, null); // refrescar
        }


        // ============================
        // EDITAR USUARIO
        // ============================
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is long id)
            {
                UsuarioFormulario form = new UsuarioFormulario(id);
                form.ShowDialog();
                UsuariosView_Loaded(null, null); // refrescar
            }
            else
            {
                MessageBox.Show("Error: no se encontró el ID del usuario.");
            }
        }




        // ============================
        // ELIMINAR USUARIO
        // ============================
        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUsuarios.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un usuario.");
                return;
            }

            var fila = (dynamic)DataGridUsuarios.SelectedItem;
            long id = fila.IdUsuario;

            if (MessageBox.Show("¿Seguro que desea eliminar este usuario?",
                "Confirmación",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                bool ok = await _service.EliminarUsuarioAsync(id);

                if (!ok)
                    MessageBox.Show("Error eliminando usuario.");
                else
                    MessageBox.Show("Usuario eliminado correctamente.");

                UsuariosView_Loaded(null, null);
            }
        }

    }
}
