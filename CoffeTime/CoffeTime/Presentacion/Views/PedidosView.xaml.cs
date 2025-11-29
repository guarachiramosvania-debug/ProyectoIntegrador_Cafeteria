using CoffeTime.Negocio.Servicios;
using System;
using System.Windows;

namespace CoffeTime.Presentacion.Views
{
    public partial class PedidosView : Window
    {
        private readonly PedidoService _service = new PedidoService();

        public PedidosView()
        {
            InitializeComponent();
            Loaded += PedidosView_Loaded;
        }

        private async void PedidosView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var lista = await _service.ObtenerPedidosAsync();

                MessageBox.Show($"Pedidos recibidos: {lista.Count}");

                icListaPedidos.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando pedidos: " + ex.Message);
            }
        }

        private void BtnNuevoPedido_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Crear pedido aún no implementado.");
        }

        private async void BtnCancelarPedido_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe &&
                fe.Tag is long id)
            {
                await _service.CancelarPedidoAsync(id);
                PedidosView_Loaded(null, null);
            }
        }
    }
}
