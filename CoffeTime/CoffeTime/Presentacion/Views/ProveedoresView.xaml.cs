using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class ProveedoresView : Window
    {
        private readonly ProveedorRepository repo = new ProveedorRepository();

        public ProveedoresView()
        {
            InitializeComponent();
            CargarProveedores();
        }

        private async void CargarProveedores()
        {
            var lista = await repo.GetAll();
            listaProveedores.ItemsSource = lista;
        }

        private async void EliminarProveedor(object sender, RoutedEventArgs e)
        {
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
            var ventana = new ProveedorFormulario(null); // null = nuevo proveedor
            if (ventana.ShowDialog() == true)
                CargarProveedores(); // refresca la lista
        }

    }
}
