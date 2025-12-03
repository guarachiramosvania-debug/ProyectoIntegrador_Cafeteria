using CoffeTime.Negocio.Modelos;
using CoffeTime.Presentacion.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using CoffeTime.Datos.Conexion;
using Supabase;
using CoffeTime.Datos.Repositorios;
using CoffeTime.Presentacion.Views;

namespace CoffeTime.Presentacion.Views
{
    public partial class DashboardView : Window
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();

            var vm = (DashboardViewModel)DataContext;

            if (vm.LoadDashboardDataCommand.CanExecute(null))
            {
                vm.LoadDashboardDataCommand.Execute(null);
            }

            MantenerUsuarioOnline();
        }

        private async void MantenerUsuarioOnline()
        {
            if (Application.Current.Properties["IdUsuario"] is long id)
            {
                await new UsuarioRepository().ActualizarOnlineSoloAsync(id, true);
            }
        }

        private void AbrirPedidosView()
        {
            try
            {
                PedidosView pedidosWindow = new PedidosView();
                pedidosWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ventana de Pedidos: " + ex.Message,
                    "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AbrirNuevoPedidoView()
        {
            try
            {
                NuevoPedidoView nuevoPedidoWindow = new NuevoPedidoView();
                nuevoPedidoWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ventana de Nuevo Pedido: " + ex.Message,
                    "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAccesoPedidos_Click(object sender, RoutedEventArgs e)
        {
            AbrirPedidosView();
        }

        private void btnNuevoPedidoRapido_Click(object sender, RoutedEventArgs e)
        {
            AbrirNuevoPedidoView();
        }

        private void btnAccesoInventario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventarioView inventarioWindow = new InventarioView();
                inventarioWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ventana de Inventario: " + ex.Message,
                    "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DashboardViewModel : ViewModelBase
    {
        private readonly Supabase.Client _client = SupabaseContext.Client;

        private decimal _ventasDelDia;
        public decimal VentasDelDia
        {
            get => _ventasDelDia;
            set { _ventasDelDia = value; OnPropertyChanged(); }
        }

        private int _pedidosDelDia;
        public int PedidosDelDia
        {
            get => _pedidosDelDia;
            set { _pedidosDelDia = value; OnPropertyChanged(); }
        }

        private int _alertasDeStock;
        public int AlertasDeStock
        {
            get => _alertasDeStock;
            set { _alertasDeStock = value; OnPropertyChanged(); }
        }

        public ICommand LoadDashboardDataCommand { get; private set; }
        public ICommand NewQuickOrderCommand { get; private set; }
        public ICommand NavigateToOrdersCommand { get; private set; }
        public ICommand NavigateToInventoryCommand { get; private set; }

        public DashboardViewModel()
        {
            LoadDashboardDataCommand = new RelayCommand(ExecuteLoadDashboardData);
            NavigateToOrdersCommand = new RelayCommand(p => { });
            NewQuickOrderCommand = new RelayCommand(p => { });
            NavigateToInventoryCommand = new RelayCommand(p => { });
        }

        private async void ExecuteLoadDashboardData(object parameter)
        {
            try
            {
                await LoadVentasPedidosAsync();
                await LoadAlertasStockAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar datos del dashboard: {ex.Message}",
                    "Error de Carga",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                VentasDelDia = 0m;
                PedidosDelDia = 0;
                AlertasDeStock = 0;
            }
        }

        private async Task LoadVentasPedidosAsync()
        {
            var response = await _client.From<Pedido>().Get();
            var todosLosPedidos = response.Models;

            var pedidosDelDiaPagados = todosLosPedidos
                .Where(p => p.Fecha.Date == DateTime.Today.Date && p.Estado == "Pagado")
                .ToList();

            VentasDelDia = pedidosDelDiaPagados.Sum(p => p.Total);
            PedidosDelDia = pedidosDelDiaPagados.Count;
        }

        private async Task LoadAlertasStockAsync(int limiteStockBajo = 5)
        {
            var response = await _client.From<Insumo>().Get();
            var insumos = response.Models;

            AlertasDeStock = insumos
                .Count(i => i.StockActual <= limiteStockBajo);
        }
    }
}
