using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using CoffeTime.Negocio.Servicios;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CoffeTime.Presentacion.Views
{
    public partial class UsuarioFormulario : Window
    {
        private readonly UsuarioRepository _repo;
        private readonly UsuarioService _service;
        private readonly long? _id;

        public UsuarioFormulario(long? idUsuario)
        {
            InitializeComponent();

            _repo = new UsuarioRepository();
            _service = new UsuarioService(_repo);
            _id = idUsuario;

            if (_id != null)
                CargarDatos();
        }

        private async void CargarDatos()
        {
            var usuarios = await _repo.ObtenerTodosAsync();
            var u = usuarios.FirstOrDefault(x => x.IdUsuario == _id);
            if (u == null) return;

            txtUsuario.Text = u.NombreUsuario;
            txtNombre.Text = u.Nombre;
            txtApellido.Text = u.Apellido;
            txtContrasena.Password = u.Contrasena;

            cmbRol.SelectedItem =
                cmbRol.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == u.Rol);
        }

        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string contrasena = txtContrasena.Password.Trim();

            if (cmbRol.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un rol.");
                return;
            }

            string rol = (cmbRol.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            if (usuario == "" || nombre == "" || apellido == "" || contrasena == "")
            {
                MessageBox.Show("Debe llenar todos los campos.");
                return;
            }

            if (_id == null)
            {
                // Crear nuevo
                var nuevo = new Usuario
                {
                    NombreUsuario = usuario,
                    Nombre = nombre,
                    Apellido = apellido,
                    Contrasena = contrasena,
                    Rol = rol,
                    Estado = true
                };

                bool ok = await _service.CrearUsuarioAsync(nuevo);

                if (ok)
                    MessageBox.Show("Usuario creado correctamente.");
                else
                    MessageBox.Show("Error al crear usuario.");
            }
            else
            {
                // Editar
                var usuarioEditado = new Usuario
                {
                    IdUsuario = _id.Value,
                    NombreUsuario = usuario,
                    Nombre = nombre,
                    Apellido = apellido,
                    Contrasena = contrasena,
                    Rol = rol,
                    Estado = true
                };

                bool ok = await _repo.ActualizarUsuarioAsync(usuarioEditado);

                if (!ok)
                    MessageBox.Show("Error actualizando usuario.");
                else
                    MessageBox.Show("Usuario actualizado correctamente.");
            }

            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    
}
