using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class ProveedorFormulario : Window
    {
        private readonly ProveedorRepository repo = new ProveedorRepository();
        private readonly UsuarioRepository usuarioRepo = new UsuarioRepository();

        private int? proveedorId = null;
        private Proveedor _proveedor;
        private readonly ProveedorRepository _repo;

        public ProveedorFormulario(int? idProveedor)
        {
            InitializeComponent();
            MantenerUsuarioOnline();
            _repo = new ProveedorRepository();

            if (idProveedor == null)
            {
                _proveedor = new Proveedor();
                Title = "Nuevo Proveedor";
            }
            else
            {
                Title = "Editar Proveedor";
                CargarProveedor(idProveedor.Value);
            }
        }

        private async void MantenerUsuarioOnline()
        {
            if (App.Current.Properties["IdUsuario"] is long id)
            {
                var usuario = await usuarioRepo.ObtenerPorIdAsync(id);

                if (usuario != null)
                {
                    usuario.Online = true;
                    await usuarioRepo.ActualizarOnlineAsync(usuario.IdUsuario, true);
                }
            }
        }

        private async void CargarProveedor(int id)
        {
            _proveedor = await _repo.GetById(id);

            txtNombre.Text = _proveedor.Nombre;
            txtContacto.Text = _proveedor.Contacto;
            txtEmail.Text = _proveedor.Email;
            txtTelefono.Text = _proveedor.Telefono;
        }


        private async void Guardo(object sender, RoutedEventArgs e)
        {
            var p = new Proveedor
            {
                Id = proveedorId ?? 0,
                Nombre = txtNombre.Text,
                Contacto = txtContacto.Text,
                Email = txtEmail.Text,
                Telefono = txtTelefono.Text
            };

            if (proveedorId == null)
                await repo.Insert(p);
            else
                await repo.Update(p);

            MessageBox.Show("Proveedor guardado correctamente");
            Close();
        }



        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            _proveedor.Nombre = txtNombre.Text;
            _proveedor.Contacto = txtContacto.Text;
            _proveedor.Email = txtEmail.Text;
            _proveedor.Telefono = txtTelefono.Text;

            bool ok = false;

            if (_proveedor.Id == 0)
                ok = await _repo.Insert(_proveedor);
            else
                ok = await _repo.Update(_proveedor);

            if (ok)
            {
                MessageBox.Show("✔ Guardado correctamente");
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("❌ Error al guardar");
            }
        }


    }
}
