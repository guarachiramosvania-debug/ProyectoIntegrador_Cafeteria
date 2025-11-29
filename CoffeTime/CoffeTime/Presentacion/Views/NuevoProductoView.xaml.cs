using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CoffeTime.Presentacion.Views
{
    public partial class NuevoProductoView : Window
    {
        private readonly ProductoRepository _productoRepo = new ProductoRepository();
        private readonly InsumoRepository _insumoRepo = new InsumoRepository();
        private readonly ProductoInsumoRepository _prodInsumoRepo = new ProductoInsumoRepository();

        private List<Insumo> _insumos = new();

        public NuevoProductoView()
        {
            InitializeComponent();
            CargarInsumos();
        }

        private async void CargarInsumos()
        {
            _insumos = await _insumoRepo.ObtenerTodosAsync();
            ListaInsumos.ItemsSource = _insumos;
        }

        private async void GuardarProducto(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text) ||
                string.IsNullOrWhiteSpace(TxtCategoria.Text) ||
                string.IsNullOrWhiteSpace(TxtPrecio.Text))
            {
                MessageBox.Show("Debe llenar todos los campos.");
                return;
            }

            var nuevo = new Producto
            {
                Nombre = TxtNombre.Text,
                Categoria = TxtCategoria.Text,
                Precio = decimal.Parse(TxtPrecio.Text),
                Descripcion = TxtDescripcion.Text,
                ImagenUrl = ""
            };

            bool ok = await _productoRepo.Insert(nuevo);
            if (!ok)
            {
                MessageBox.Show("Error guardando producto.");
                return;
            }

            // Obtener ID recién insertado
            var productos = await _productoRepo.GetAll();
            var creado = productos.LastOrDefault(p => p.Nombre == nuevo.Nombre);

            if (creado == null)
            {
                MessageBox.Show("No se pudo obtener ID.");
                return;
            }

            int idProducto = creado.Id;

            // Recorrer insumos
            foreach (var item in ListaInsumos.Items)
            {
                var insumo = item as Insumo;
                var container = ListaInsumos.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;

                var txtCantidad = container?.FindName("CantidadBox") as TextBox;

                if (txtCantidad == null || string.IsNullOrWhiteSpace(txtCantidad.Text))
                    continue;

                if (!decimal.TryParse(txtCantidad.Text, out var cantidad))
                    continue;

                var pi = new ProductoInsumo
                {
                    IdProducto = idProducto,
                    IdInsumo = insumo.IdInsumo,
                    Cantidad = cantidad
                };

                await _prodInsumoRepo.Insert(pi);
            }


            MessageBox.Show("Producto registrado correctamente.");
            Close();
        }
    }
}
