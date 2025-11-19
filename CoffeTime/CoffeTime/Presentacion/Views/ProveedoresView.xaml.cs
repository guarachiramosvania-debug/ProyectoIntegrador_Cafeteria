using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.Windows;
using System.Windows.Controls;

namespace CoffeTime.Presentacion.Views
{
    public partial class ProveedoresView : UserControl
    {
        private readonly ProveedorRepository _repo = new ProveedorRepository();

        public ProveedoresView()
        {
            InitializeComponent();
            CargarProveedores();
        }

        private async void CargarProveedores()
        {
            var lista = await _repo.GetAll();
            ListaProveedores.ItemsSource = lista;
        }

        private void NuevoProveedor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abrir formulario de Nuevo Proveedor");
            // Aquí después se agregará un formulario real
        }

        private async void EliminarProveedor_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(((Button)sender).Tag.ToString());

            if (MessageBox.Show("¿Eliminar proveedor?", "Confirmar",
               MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _repo.Delete(id);
                CargarProveedores();
            }
        }

        private void EditarProveedor_Click(object sender, RoutedEventArgs e)
        {
            var id = int.Parse(((Button)sender).Tag.ToString());
            MessageBox.Show($"Editar proveedor ID: {id}");
            // Aquí luego abrimos un form de edición
        }
    }
}
