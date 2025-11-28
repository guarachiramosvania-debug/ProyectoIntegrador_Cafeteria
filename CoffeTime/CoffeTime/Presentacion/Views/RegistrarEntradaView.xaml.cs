using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class RegistrarEntradaView : Window
    {
        private readonly List<Insumo> _insumos;

        public RegistrarEntradaView(List<Insumo> insumos)
        {
            InitializeComponent();
            _insumos = insumos ?? new List<Insumo>();
            CmbInsumo.ItemsSource = _insumos;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (CmbInsumo.SelectedItem is not Insumo insumo)
            {
                MessageBox.Show("Debe seleccionar un insumo.");
                return;
            }

            if (!decimal.TryParse(TxtCantidad.Text.Replace(',', '.'),
                                  NumberStyles.Any,
                                  CultureInfo.InvariantCulture,
                                  out decimal cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Cantidad inválida.");
                return;
            }

            decimal? costoTotal = null;
            if (!string.IsNullOrWhiteSpace(TxtCostoTotal.Text))
            {
                if (!decimal.TryParse(TxtCostoTotal.Text.Replace(',', '.'),
                                      NumberStyles.Any,
                                      CultureInfo.InvariantCulture,
                                      out decimal costo))
                {
                    MessageBox.Show("Costo total inválido.");
                    return;
                }
                costoTotal = costo;
            }

            if (App.Current.Properties["IdUsuario"] is not long idUsuario)
            {
                MessageBox.Show("No se encontró el usuario actual.");
                return;
            }

            var insumoRepo = new InsumoRepository();
            var movRepo = new MovimientoInventarioRepository();

            // 1) Actualizar stock
            var nuevoStock = insumo.StockActual + cantidad;
            bool okStock = await insumoRepo.ActualizarStock(insumo.IdInsumo, nuevoStock);

            // 2) Registrar movimiento
            var movimiento = new MovimientoInventario
            {
                IdInsumo = insumo.IdInsumo,
                TipoMovimiento = "entrada",
                Cantidad = cantidad,
                Fecha = DateTime.Now,
                UsuarioResponsable = idUsuario,
                CostoTotal = costoTotal
            };

            bool okMov = await movRepo.RegistrarMovimientoAsync(movimiento);

            if (!okStock || !okMov)
            {
                MessageBox.Show("Ocurrió un error al registrar la entrada.");
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
