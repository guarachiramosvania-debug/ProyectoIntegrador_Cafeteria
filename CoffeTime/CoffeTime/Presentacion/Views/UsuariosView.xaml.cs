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
        private readonly UsuarioRepository usuarioRepo = new UsuarioRepository();

        private long? _idSeleccionado = null;

        protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            await ((App)Application.Current).CerrarSesionAutomatica();
            base.OnClosing(e);
        }

        public UsuariosView()
        {
            InitializeComponent();
            _service = new UsuarioService(new UsuarioRepository());

            MantenerUsuarioOnline();  // ⭐ importante
            Loaded += UsuariosView_Loaded;
        }

        // ============================================================
        // MANTENER AL USUARIO ACTUAL COMO ONLINE (pero solo una vez)
        // ============================================================
        private async void MantenerUsuarioOnline()
        {
            if (App.Current.Properties["IdUsuario"] is long id)
            {
                await usuarioRepo.ActualizarOnlineAsync(id, true);
            }
        }

        // ============================================================
        // CARGAR LISTADO DE USUARIOS (FIX PARA ONLINE REAL)
        // ============================================================
        private async void UsuariosView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var usuarios = await _service.ObtenerTodosAsync();

                long idActual = (long)App.Current.Properties["IdUsuario"];

                // ⭐ FORZAR ONLINE PARA EL USUARIO ACTUAL SIN ESPERAR A SUPABASE
                foreach (var u in usuarios)
                {
                    if (u.IdUsuario == idActual)
                    {
                        u.Online = true;  // fuerza visual ✔
                    }
                }

                var lista = usuarios.Select(u => new
                {
                    IdUsuario = u.IdUsuario,
                    u.NombreUsuario,
                    NombreCompleto = $"{u.Nombre} {u.Apellido}",
                    Rol = u.Rol,

                    Estado = (u.Online ?? false) ? "🟢 Online" : "⚫ Offline",

                    UltimoLogin = u.UltimoLogin == null
                                  ? "—"
                                  : u.UltimoLogin.Value.ToString("dd/MM/yyyy HH:mm")
                })
                .OrderBy(u => u.Rol)      // opcional
                .ThenBy(u => u.NombreUsuario)
                .ToList();

                DataGridUsuarios.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message);
            }
        }

        // ============================================================
        // NUEVO USUARIO
        // ============================================================
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            UsuarioFormulario form = new UsuarioFormulario(null);
            form.ShowDialog();
            UsuariosView_Loaded(null, null);
        }

        // ============================================================
        // EDITAR USUARIO
        // ============================================================
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is long id)
            {
                UsuarioFormulario form = new UsuarioFormulario(id);
                form.ShowDialog();
                UsuariosView_Loaded(null, null);
            }
            else
            {
                MessageBox.Show("Error: no se encontró el ID del usuario.");
            }
        }

        // ============================================================
        // ELIMINAR USUARIO
        // ============================================================
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
