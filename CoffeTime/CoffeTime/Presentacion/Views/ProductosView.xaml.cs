using CoffeTime.Datos.Repositorios;
using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class ProductosView : Window
    {
        private readonly ProductoRepository repo = new ProductoRepository();
        private readonly UsuarioRepository usuarioRepo = new UsuarioRepository();

        public ProductosView()
        {
            InitializeComponent();
            MantenerUsuarioOnline();
            CargarProductos();

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
        

        private async void CargarProductos()
        {
            var lista = await repo.GetAll();
            listaProductos.ItemsSource = lista;
        }

        private void BtnNuevoProducto(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aquí abrirás formulario nuevo producto.");
        }

        private void EditarProducto(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as FrameworkElement).Tag.ToString());
            MessageBox.Show($"Editar producto ID: {id}");
        }

        private async void EliminarProducto(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as FrameworkElement).Tag.ToString());

            if (MessageBox.Show("¿Eliminar este producto?",
                "Confirmación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await repo.Delete(id);
                CargarProductos();
            }
        }
    }
}
