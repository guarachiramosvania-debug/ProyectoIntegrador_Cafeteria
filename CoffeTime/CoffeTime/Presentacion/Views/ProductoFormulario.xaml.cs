using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.IO;
using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class ProductoFormulario : Window
    {
        private readonly ProductoRepository _repo = new ProductoRepository();
        private readonly int? _id;
        private string _rutaImagenSeleccionada;

        public ProductoFormulario(int? id = null)
        {
            InitializeComponent();
            _id = id;

            if (_id != null)
                CargarDatos();
        }

        private async void CargarDatos()
        {
            var p = await _repo.GetById(_id.Value);
            if (p == null) return;

            txtNombre.Text = p.Nombre;
            txtCategoria.Text = p.Categoria;
            txtPrecio.Text = p.Precio.ToString();
            txtDescripcion.Text = p.Descripcion;

            if (!string.IsNullOrEmpty(p.ImagenUrl))
            {
                _rutaImagenSeleccionada = p.ImagenUrl;
                imgPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(p.ImagenUrl, UriKind.RelativeOrAbsolute));
            }
        }

        private void BtnSeleccionarImagen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Imágenes|*.jpg;*.jpeg;*.png";

            if (dlg.ShowDialog() == true)
            {
                _rutaImagenSeleccionada = dlg.FileName;
                imgPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(dlg.FileName));
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Precio inválido");
                return;
            }

            string imagenFinal = null;

            // GUARDAR IMAGEN EN CARPETA INTERNA
            if (!string.IsNullOrEmpty(_rutaImagenSeleccionada))
            {
                string carpeta = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Presentacion", "Assets", "Productos");

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string nombreImg = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(_rutaImagenSeleccionada)}";
                string destino = System.IO.Path.Combine(carpeta, nombreImg);

                File.Copy(_rutaImagenSeleccionada, destino, true);

                imagenFinal = destino;
            }

            var producto = new Producto
            {
                Id = _id ?? 0,
                Nombre = txtNombre.Text,
                Categoria = txtCategoria.Text,
                Precio = precio,
                Descripcion = txtDescripcion.Text,
                ImagenUrl = imagenFinal
            };

            bool ok;

            if (_id == null)
                ok = await _repo.Insert(producto);
            else
                ok = await _repo.Update(producto);

            if (!ok)
                MessageBox.Show("Error al guardar.");
            else
                DialogResult = true;

            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
