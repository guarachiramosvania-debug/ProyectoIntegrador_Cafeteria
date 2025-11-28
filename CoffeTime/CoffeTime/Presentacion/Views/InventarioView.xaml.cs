using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CoffeTime.Presentacion.Views
{
    public partial class InventarioView : Window
    {
        private List<Insumo> _insumosBase = new();
        private List<MovimientoInventario> _historialBase = new();
        private List<Proveedor> _proveedoresBase = new();

        public InventarioView()
        {
            InitializeComponent();
            Loaded += async (_, __) => await CargarDatosAsync();
            MantenerUsuarioOnline();
        }

        // =========================
        // ONLINE
        // =========================
        private async void MantenerUsuarioOnline()
        {
            try
            {
                if (App.Current.Properties["IdUsuario"] is long id)
                {
                    var repo = new UsuarioRepository();
                    await repo.ActualizarOnlineSoloAsync(id, true);
                }
            }
            catch { }
        }

        // =========================
        // CARGA INICIAL
        // =========================
        private async void InventarioView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var insumoRepo = new InsumoRepository();
                var proveedorRepo = new ProveedorRepository();

                var insumos = await insumoRepo.ObtenerTodosAsync();

                var lista = new List<object>();

                foreach (var insumo in insumos)
                {
                    Proveedor? proveedor = null;

                    if (insumo.IdProveedor.HasValue)
                        proveedor = await proveedorRepo.GetById(insumo.IdProveedor.Value);

                    lista.Add(new
                    {
                        Nombre = insumo.Nombre,
                        Proveedor = proveedor?.Nombre ?? "—",
                        Stock = insumo.StockActual,
                        Minimo = insumo.StockMinimo,
                        Estado = insumo.StockActual < insumo.StockMinimo ? "Alerta" : "OK"
                    });
                }

                DataGridInventarioActual.ItemsSource = lista;

                // HISTORIAL
                var movRepo = new MovimientoInventarioRepository();
                var historial = await movRepo.ObtenerHistorialAsync();

                MostrarHistorial(_historialBase);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR Inventario: {ex.Message}");
            }
        }


        private async Task CargarDatosAsync()
        {
            try
            {
                var insumoRepo = new InsumoRepository();
                var movRepo = new MovimientoInventarioRepository();
                var provRepo = new ProveedorRepository();   // AGREGAR
                var usuarioRepo = new UsuarioRepository();

                // Traemos todo
                _insumosBase = await insumoRepo.ObtenerTodosAsync();
                _historialBase = await movRepo.ObtenerHistorialAsync();
                _proveedoresBase = await provRepo.GetAll();   // AGREGAR

                // Refrescamos grilla principal
                RefrescarTablaInsumos();

                // Diccionarios para proveedores
                var dicInsumos = _insumosBase.ToDictionary(i => i.IdInsumo, i => i);
                var dicUsuarios = (await usuarioRepo.ObtenerTodosAsync())
                                  .ToDictionary(u => u.IdUsuario, u => u.NombreUsuario);

                MostrarHistorial(_historialBase);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR Inventario: {ex.Message}");
            }
        }


        // =========================
        // TABLA PRINCIPAL + BÚSQUEDA
        // =========================
        private void RefrescarTablaInsumos()
        {
            DataGridInventarioActual.ItemsSource = _insumosBase.Select(i =>
            {
                // Obtener proveedor
                var proveedor = _proveedoresBase.FirstOrDefault(p => p.Id == i.IdProveedor);

                return new
                {
                    Nombre = i.Nombre,
                    Proveedor = proveedor?.Nombre ?? "—",
                    Stock = i.StockActual,
                    Minimo = i.StockMinimo,
                    Estado = i.StockActual < i.StockMinimo ? "Alerta" : "OK"
                };
            }).ToList();
        }


        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefrescarTablaInsumos();
        }

        // =========================
        // HISTORIAL DETALLADO
        // =========================
        private async Task MostrarHistorial(List<MovimientoInventario> movimientos)
        {
            var insumoRepo = new InsumoRepository();
            var proveedorRepo = new ProveedorRepository();

            StackHistorial.Children.Clear();

            foreach (var m in movimientos)
            {
                var insumo = await insumoRepo.ObtenerPorId(m.IdInsumo);
                Proveedor? proveedor = null;

                if (insumo?.IdProveedor != null)
                    proveedor = await proveedorRepo.GetById(insumo.IdProveedor.Value);

                var card = new Border
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(10),
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(8),
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1)
                };

                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text = $"{m.Fecha:dd/MM/yyyy HH:mm}",
                    FontWeight = FontWeights.Bold
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Insumo: {insumo?.Nombre}"
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Proveedor: {proveedor?.Nombre ?? "—"}"
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Cantidad: +{m.Cantidad}"
                });

                card.Child = stack;
                StackHistorial.Children.Add(card);
            }
        }



        // =========================
        // REGISTRAR ENTRADA
        // =========================
        private async void BtnRegistrarEntrada_Click(object sender, RoutedEventArgs e)
        {
          

            var ventana = new RegistrarEntradaView(_insumosBase)
            {
                Owner = this
            };

            var result = ventana.ShowDialog();
            if (result == true)
            {
                // Se guardó una entrada → recargamos datos
                await CargarDatosAsync();
            }
        }
    }
}
