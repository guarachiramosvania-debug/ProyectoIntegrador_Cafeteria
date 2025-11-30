using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CoffeTime.Presentacion.Views
{
    public partial class ProductosView : Window
    {
        // Repositorio
        private readonly ProductoRepository _repo = new ProductoRepository();

        // Lista base (sin filtros)
        private List<Producto> _productosBase = new();

        // ID del producto que se está editando
        private int _idEditando = -1;

        public ProductosView()
        {
            InitializeComponent();
            CargarProductos();
        }

        // ======================================================
        //   CARGAR DATOS
        // ======================================================
        private async void CargarProductos()
        {
            try
            {
                _productosBase = await _repo.GetAll();

                // Categorías para filtro
                var categorias = _productosBase
                    .Select(p => p.Categoria)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                categorias.Insert(0, "Todas");
                CmbCategorias.ItemsSource = categorias;
                CmbCategorias.SelectedIndex = 0;

                ListaProductos.ItemsSource = _productosBase;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR cargando productos: " + ex.Message);
            }
        }

        // ======================================================
        //   BUSCADOR + PLACEHOLDER
        // ======================================================
        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Mostrar u ocultar placeholder
            TxtBuscarPlaceholder.Visibility = string.IsNullOrWhiteSpace(TxtBuscar.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            FiltrarProductos();
        }

        private void CmbCategorias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FiltrarProductos();
        }

        // ======================================================
        //   FILTRO
        // ======================================================
        private void FiltrarProductos()
        {
            string txt = TxtBuscar.Text.ToLower();
            string categoria = CmbCategorias.SelectedItem?.ToString() ?? "Todas";

            var filtrado = _productosBase.Where(p =>
                (string.IsNullOrWhiteSpace(txt) || p.Nombre.ToLower().Contains(txt))
                &&
                (categoria == "Todas" || p.Categoria == categoria)
            ).ToList();

            ListaProductos.ItemsSource = filtrado;
        }

        // ======================================================
        //   NUEVO PRODUCTO
        // ======================================================
        private void BtnNuevoProducto(object sender, RoutedEventArgs e)
        {

            if (!PermisosService.EsAdmin())
            {
                MessageBox.Show("No tienes permisos para acceder a esta sección.");
                this.Tag = "DENIED"; // marcar que NO debe abrirse
                return;
            }
            var win = new NuevoProductoView();
            win.ShowDialog();
            CargarProductos(); // refresca lista
        }


        // ======================================================
        //   EDITAR PRODUCTO
        // ======================================================
        private async void EditarProducto(object sender, RoutedEventArgs e)
        {

            if (!PermisosService.EsAdmin())
            {
                MessageBox.Show("No tienes permisos para acceder a esta sección.");
                this.Tag = "DENIED"; // marcar que NO debe abrirse
                return;
            }
            try
            {
                _idEditando = int.Parse((sender as FrameworkElement).Tag.ToString());
                var p = await _repo.GetById(_idEditando);

                if (p == null)
                {
                    MessageBox.Show("Error: el producto no existe.");
                    return;
                }

                TxtNombreEdit.Text = p.Nombre;
                TxtCategoriaEdit.Text = p.Categoria;
                TxtPrecioEdit.Text = p.Precio.ToString();
                TxtDescripcionEdit.Text = p.Descripcion;

                ModalFondo.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR al cargar datos de edición: " + ex.Message);
            }
        }

        // ======================================================
        //   GUARDAR EDICIÓN
        // ======================================================
        private async void GuardarEdicion(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener datos actuales del producto
                var original = await _repo.GetById(_idEditando);
                if (original == null)
                {
                    MessageBox.Show("Error: producto no encontrado.");
                    return;
                }

                // Crear objeto actualizado conservando la imagen
                var p = new Producto
                {
                    Id = _idEditando,
                    Nombre = TxtNombreEdit.Text,
                    Categoria = TxtCategoriaEdit.Text,
                    Precio = decimal.Parse(TxtPrecioEdit.Text),
                    Descripcion = TxtDescripcionEdit.Text,
                    ImagenUrl = original.ImagenUrl   // ✔ conservar
                };

                await _repo.Update(p);

                ModalFondo.Visibility = Visibility.Collapsed;
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR al guardar cambios: " + ex.Message);
            }
        }


        // ======================================================
        //   CERRAR MODAL
        // ======================================================
        private void CerrarModal(object sender, RoutedEventArgs e)
        {
            ModalFondo.Visibility = Visibility.Collapsed;
        }


        // ======================================================
        //   ELIMINAR PRODUCTO
        // ======================================================
        private async void EliminarProducto(object sender, RoutedEventArgs e)
        {
            if (!PermisosService.EsAdmin())
            {
                MessageBox.Show("No tienes permisos para acceder a esta sección.");
                this.Tag = "DENIED"; // marcar que NO debe abrirse
                return;
            }
            try
            {
                int id = int.Parse((sender as FrameworkElement).Tag.ToString());

                if (MessageBox.Show("¿Eliminar este producto?",
                                    "Confirmación",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning)
                                    == MessageBoxResult.Yes)
                {
                    await _repo.Delete(id);
                    CargarProductos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR eliminando producto: " + ex.Message);
            }
        }

        // ======================================================
        //   ANIMACIÓN DEL EXPANDER
        // ======================================================
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is Expander expander)
            {
                var content = expander.Content as FrameworkElement;
                if (content == null) return;

                DoubleAnimation anim = new DoubleAnimation
                {
                    From = 0,
                    To = content.ActualHeight,
                    Duration = TimeSpan.FromMilliseconds(250),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                content.BeginAnimation(FrameworkElement.HeightProperty, anim);
            }
        }
        private void FiltrarProductos(object sender, SelectionChangedEventArgs e)
        {
            FiltrarProductos();
        }

    }
}
