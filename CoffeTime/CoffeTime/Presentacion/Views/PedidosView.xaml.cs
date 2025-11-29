using CoffeTime.Negocio.Servicios;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CoffeTime.Presentacion.Views
{
    public partial class PedidosView : Window
    {
        private readonly PedidoService _service = new PedidoService();

        public PedidosView()
        {
            InitializeComponent();
            Loaded += async (_, __) => await CargarPedidosAsync();
        }

        // ======================================================
        // CARGAR PEDIDOS (llamar SIEMPRE después de cambios)
        // ======================================================
        private async Task CargarPedidosAsync(string estado = "Todos")
        {
            var lista = await _service.ObtenerPedidosAsync();

            if (estado != "Todos")
                lista = lista.Where(x => x.Estado == estado).ToList();

            icListaPedidos.ItemsSource = lista;
        }

        // ======================================================
        // FILTRO DE PEDIDOS (Pendientes, Pagados, Cancelados)
        // ======================================================
        private async void FiltroChanged(object sender, SelectionChangedEventArgs e)
        {
            string filtro = (CmbFiltro.SelectedItem as ComboBoxItem)?.Content.ToString();
            await CargarPedidosAsync(filtro);
        }

        // ======================================================
        // PAGAR PEDIDO
        // ======================================================
        private async void BtnPagarPedido_Click(object sender, RoutedEventArgs e)
        {
            long id = (long)(sender as FrameworkElement).Tag;
            await _service.PagarPedidoRPC(id);

            string filtro = (CmbFiltro.SelectedItem as ComboBoxItem)?.Content.ToString();
            await CargarPedidosAsync(filtro);
        }

        // ======================================================
        // CANCELAR PEDIDO
        // ======================================================
        private async void BtnCancelarPedido_Click(object sender, RoutedEventArgs e)
        {
            long id = (long)(sender as FrameworkElement).Tag;
            await _service.CancelarPedidoAsync(id);

            string filtro = (CmbFiltro.SelectedItem as ComboBoxItem)?.Content.ToString();
            await CargarPedidosAsync(filtro);
        }

        // ======================================================
        // NUEVO PEDIDO
        // ======================================================
        private async void BtnNuevoPedido_Click(object sender, RoutedEventArgs e)
        {
            var win = new NuevoPedidoView();
            bool? result = win.ShowDialog();

            if (result == true)
                await CargarPedidosAsync();
        }
    }
}
