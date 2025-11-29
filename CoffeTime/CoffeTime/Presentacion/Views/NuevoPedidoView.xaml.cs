using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Servicios;
using CoffeTime.Negocio.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CoffeTime.Presentacion.Views
{
    public partial class NuevoPedidoView : Window
    {
        private readonly ProductoRepository _productoRepo = new ProductoRepository();
        private readonly PedidoService _pedidoService = new PedidoService();
        private readonly UsuarioRepository _usuarioRepo = new UsuarioRepository();

        private List<Producto> _productosBase = new();
        private List<DetalleTemp> _detalle = new();

        public NuevoPedidoView()
        {
            InitializeComponent();
            MantenerUsuarioOnline();
            CargarProductos();
        }

        // ======================================================
        //   MANTENER USUARIO ONLINE  (SOLUCIÓN AL PROBLEMA)
        // ======================================================
        private async void MantenerUsuarioOnline()
        {
            if (App.Current.Properties["IdUsuario"] is long id)
            {
                var usuario = await _usuarioRepo.ObtenerPorIdAsync(id);

                if (usuario != null)
                {
                    usuario.Online = true;
                    await _usuarioRepo.ActualizarOnlineAsync(usuario.IdUsuario, true);
                }
            }
        }

        // ======================================================
        //   CARGAR PRODUCTOS
        // ======================================================
        private async void CargarProductos()
        {
            _productosBase = await _productoRepo.GetAll();
            dgProductos.ItemsSource = _productosBase;
        }

        // ======================================================
        //   BUSCAR PRODUCTO
        // ======================================================
        private void TxtBuscarProducto_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txt = TxtBuscarProducto.Text.ToLower();

            dgProductos.ItemsSource = _productosBase
                .Where(p => p.Nombre.ToLower().Contains(txt))
                .ToList();
        }

        // ======================================================
        //   AGREGAR PRODUCTO AL PEDIDO
        // ======================================================
        private void BtnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is int idProd)
            {
                var prod = _productosBase.FirstOrDefault(p => p.Id == idProd);
                if (prod == null) return;

                var existente = _detalle.FirstOrDefault(d => d.IdProducto == idProd);

                if (existente == null)
                {
                    _detalle.Add(new DetalleTemp
                    {
                        IdProducto = prod.Id,
                        NombreProducto = prod.Nombre,
                        Cantidad = 1,
                        Precio = prod.Precio,
                        Subtotal = prod.Precio
                    });
                }
                else
                {
                    existente.Cantidad++;
                    existente.Subtotal = existente.Cantidad * existente.Precio;
                }

                ActualizarTabla();
            }
        }

        private void ActualizarTabla()
        {
            dgDetalle.ItemsSource = null;
            dgDetalle.ItemsSource = _detalle;

            TxtTotal.Text = "$" + _detalle.Sum(d => d.Subtotal).ToString("0.00");
        }

        // ======================================================
        //   GUARDAR PEDIDO
        // ======================================================
        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_detalle.Count == 0)
                {
                    MessageBox.Show("Agregue productos al pedido.");
                    return;
                }

                if (App.Current.Properties["IdUsuario"] is not long idUsuario)
                {
                    MessageBox.Show("No se encontró el usuario actual.");
                    return;
                }

                var metodoPago = ((ComboBoxItem)CmbMetodoPago.SelectedItem).Content.ToString();

                var items = _detalle.Select(d => (d.IdProducto, d.Cantidad)).ToList();

                bool ok = await _pedidoService.CrearPedidoAsync(metodoPago, idUsuario, items);

                if (ok)
                {
                    MessageBox.Show("Pedido registrado correctamente.");

                    // Muy importante: esto le avisa a la ventana que llamó ShowDialog()
                    this.DialogResult = true;

                    this.Close();
                }

                else
                {
                    MessageBox.Show("Error al registrar pedido.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class DetalleTemp
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal { get; set; }
    }
}
