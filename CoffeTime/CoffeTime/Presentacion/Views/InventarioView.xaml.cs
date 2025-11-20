using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CoffeTime.Presentacion.Views
{
    public partial class InventarioView : Window
    {
        public InventarioView()
        {
            InitializeComponent();

            // Intentar cargar datos (si falla, no rompe la app)
            try
            {
                CargarDatosInmediatamente();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los datos automáticamente: " + ex.Message);
            }
        }

        private void CargarDatosInmediatamente()
        {
            // Simular inventario actual (porque no tienes tabla 'insumos' aún)
            var inventario = new[]
            {
                new { Nombre = "Café Molido", Stock = "5000 g", Minimo = "1000 g", Estado = "OK" },
                new { Nombre = "Leche", Stock = "8000 ml", Minimo = "2000 ml", Estado = "OK" },
                new { Nombre = "Chocolate", Stock = "3000 g", Minimo = "500 g", Estado = "OK" },
                new { Nombre = "Croissant", Stock = "50 u", Minimo = "10 u", Estado = "OK" }
            };

            DataGridInventarioActual.ItemsSource = inventario;

            // Cargar historial desde Supabase (usando Task.Result de forma segura)
            try
            {
                var repo = new MovimientoInventarioRepository(App.SupabaseClient);
                var historialTask = repo.ObtenerHistorialAsync();
                var historial = historialTask.Result; // Este puede fallar si no hay conexión

                MostrarHistorial(historial);
            }
            catch (Exception ex)
            {
                // Mostrar error solo en consola o mensaje opcional
                System.Diagnostics.Debug.WriteLine("Error al cargar historial: " + ex.Message);
            }
        }

        private void MostrarHistorial(System.Collections.Generic.List<MovimientoInventario> movimientos)
        {
            StackHistorial.Children.Clear();

            foreach (var m in movimientos)
            {
                var card = new Border
                {
                    Style = (Style)FindResource("CardStyle"),
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(10)
                };

                var stack = new StackPanel();

                // Header
                var header = new StackPanel { Orientation = Orientation.Horizontal };
                header.Children.Add(new TextBlock { Text = "📈", FontSize = 16, Margin = new Thickness(0, 0, 8, 0) });
                header.Children.Add(new TextBlock { Text = $"Insumo {m.IdInsumo}", FontWeight = FontWeights.Bold });

                // Detalles
                var details = new StackPanel { Margin = new Thickness(0, 5, 0, 0) };
                details.Children.Add(new TextBlock { Text = $"+{m.Cantidad} unidades", Foreground = Brushes.Green });
                details.Children.Add(new TextBlock { Text = "Café Proveedor S.A.", FontStyle = FontStyles.Italic, FontSize = 12 });
                details.Children.Add(new TextBlock { Text = m.Fecha.ToString("dd/MM/yyyy"), HorizontalAlignment = HorizontalAlignment.Right, FontSize = 11 });

                // Costo
                var costo = new TextBlock
                {
                    Text = $"Bs. {m.CostoTotal?.ToString("F2") ?? "0.00"}",
                    FontWeight = FontWeights.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                stack.Children.Add(header);
                stack.Children.Add(details);
                stack.Children.Add(costo);

                card.Child = stack;
                StackHistorial.Children.Add(card);
            }
        }

        private void BtnRegistrarEntrada_Click(object sender, RoutedEventArgs e)
        {
            // Más adelante: abrir formulario
            MessageBox.Show("Registrar nueva entrada de inventario");
        }
    }
}