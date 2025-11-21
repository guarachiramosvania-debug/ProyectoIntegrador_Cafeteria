using System.Linq;
using System.Windows;
using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CoffeTime.Presentacion.Views
{
    public partial class InventarioView : Window
    {
        public InventarioView()
        {
            InitializeComponent();
            Loaded += InventarioView_Loaded;
        }

        private async void InventarioView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // PRIMER REPO: INSUMOS
                var insumoRepo = new InsumoRepository();
                var insumos = await insumoRepo.ObtenerTodosAsync();

                var lista = insumos.Select(x => new
                {
                    Nombre = x.Nombre,
                    Stock = x.StockActual,
                    Minimo = x.StockMinimo,
                    Estado = x.StockActual < x.StockMinimo ? "Alerta" : "OK"
                }).ToList();

                DataGridInventarioActual.ItemsSource = lista;

                // SEGUNDO REPO: HISTORIAL
                var movRepo = new MovimientoInventarioRepository();
                var historial = await movRepo.ObtenerHistorialAsync();

                MostrarHistorial(historial);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR Inventario: {ex.Message}");
            }
        }

        private void MostrarHistorial(List<MovimientoInventario> movimientos)
        {
            StackHistorial.Children.Clear();

            foreach (var m in movimientos)
            {
                var card = new Border
                {
                    Margin = new Thickness(0, 0, 0, 10),
                    Padding = new Thickness(10)
                };

                var stack = new StackPanel();
                stack.Children.Add(new TextBlock { Text = $"{m.Fecha:dd/MM} - +{m.Cantidad}" });

                card.Child = stack;
                StackHistorial.Children.Add(card);
            }
      
        
        
        }


        private void BtnRegistrarEntrada_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Registrar nueva entrada de inventario (NO IMPLEMENTADO)");
        }

    }
}
