using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using CoffeTime.Datos.Repositorios;

using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class ProveedoresView : Window
    {
        private readonly ProveedorRepository repo = new ProveedorRepository();
        private readonly UsuarioRepository usuarioRepo = new UsuarioRepository();


        public ProveedoresView()
        {
            InitializeComponent();
            CargarProveedores();
            MantenerUsuarioOnline();
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


        private async void CargarProveedores()
        {
            var lista = await repo.GetAll();
            listaProveedores.ItemsSource = lista;
        }

        private async void EliminarProveedor(object sender, RoutedEventArgs e)
        {
            if (!PermisosService.EsAdmin())
            {
                MessageBox.Show("No tienes permisos para acceder a esta sección.");
                this.Tag = "DENIED"; // marcar que NO debe abrirse
                return;
            }
            int id = int.Parse((sender as FrameworkElement).Tag.ToString());

            if (MessageBox.Show("¿Deseas eliminar este proveedor?",
                                "Confirmar",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await repo.Delete(id);
                CargarProveedores();
            }
        }

        private void EditarProveedor(object sender, RoutedEventArgs e)
        {
            if (!PermisosService.EsAdmin())
            {
                MessageBox.Show("No tienes permisos para acceder a esta sección.");
                this.Tag = "DENIED"; // marcar que NO debe abrirse
                return;
            }
            int id = int.Parse((sender as FrameworkElement).Tag.ToString());
            var ventana = new ProveedorFormulario(id);
            ventana.ShowDialog();
            CargarProveedores();
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new ProveedorFormulario(null);
            ventana.ShowDialog();
            CargarProveedores();
        }

        private void BtnNuevoProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (!PermisosService.EsAdmin())
            {
                MessageBox.Show("No tienes permisos para acceder a esta sección.");
                this.Tag = "DENIED"; // marcar que NO debe abrirse
                return;
            }
            var ventana = new ProveedorFormulario(null); // null = nuevo proveedor
            if (ventana.ShowDialog() == true)
                CargarProveedores(); // refresca la lista
        }

    }
}
