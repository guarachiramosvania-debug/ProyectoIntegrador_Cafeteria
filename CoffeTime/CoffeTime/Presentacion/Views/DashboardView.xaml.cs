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
using CoffeTime.Datos.Repositorios; // Asegúrate de que existe UsuarioRepository

namespace CoffeTime.Presentacion.Views
{
    // =========================================================
    // CODE-BEHIND DE LA VISTA (DashboardView.xaml.cs)
    // =========================================================
    public partial class DashboardView : Window
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();

            var vm = (DashboardViewModel)DataContext;

            // Ejecutar el comando de carga de datos al iniciar
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
                // ** Asegúrate de que UsuarioRepository existe **
                await new UsuarioRepository().ActualizarOnlineSoloAsync(id, true);
            }
        }

        // 🎯 1. NAVEGACIÓN A PEDIDOS (Acceso Rápido y Nuevo Pedido Rápido)
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
                MessageBox.Show("Error al abrir la ventana de Pedidos: " + ex.Message, "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAccesoPedidos_Click(object sender, RoutedEventArgs e)
        {
            AbrirPedidosView();
        }

        private void btnNuevoPedidoRapido_Click(object sender, RoutedEventArgs e)
        {
            AbrirPedidosView();
        }


        // 🎯 2. NAVEGACIÓN A INVENTARIO (Acceso Rápido)
        private void btnAccesoInventario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Asumiendo que tienes una ventana llamada InventarioView
                InventarioView inventarioWindow = new InventarioView();
                inventarioWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ventana de Inventario: " + ex.Message, "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


    // =========================================================
    // BASE DE VIEWMODEL
    // =========================================================
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
        
        // Conexión directa a Supabase
        
        private readonly Supabase.Client _client = SupabaseContext.Client;

        
        // Propiedades de Datos (Dashboard Cards)
        
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

            // Comandos de navegación se mantienen enlazados aunque la lógica esté en el Code-Behind
            NavigateToOrdersCommand = new RelayCommand(p => { /* Lógica en Code-Behind */ });
            NewQuickOrderCommand = new RelayCommand(p => { /* Lógica en Code-Behind */ });
            NavigateToInventoryCommand = new RelayCommand(p => { /* Lógica en Code-Behind */ });
        }


        private async void ExecuteLoadDashboardData(object parameter)
        {
            try
            {
                // 1. Cargar Ventas y Pedidos del Día
                await LoadVentasPedidosAsync();

                // 2. Cargar Alertas de Stock
                await LoadAlertasStockAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos del dashboard: {ex.Message}", "Error de Carga", MessageBoxButton.OK, MessageBoxImage.Error);
                VentasDelDia = 0m;
                PedidosDelDia = 0;
                AlertasDeStock = 0;
            }
        }

        // Lógica para obtener ventas y pedidos usando Supabase.Client directamente
        private async Task LoadVentasPedidosAsync()
        {
            // Obtener todos los pedidos
            var response = await _client.From<Pedido>().Get();
            var todosLosPedidos = response.Models;

            // Filtrar por la fecha de hoy y estado "Pagado"
            var pedidosDelDiaPagados = todosLosPedidos
                .Where(p => p.Fecha.Date == DateTime.Today.Date && p.Estado == "Pagado")
                .ToList();

            // Usamos la propiedad 'Total' según tu PedidoService
            VentasDelDia = pedidosDelDiaPagados.Sum(p => p.Total);
            PedidosDelDia = pedidosDelDiaPagados.Count;
        }

        // Lógica para obtener alertas de stock usando Supabase.Client directamente
        private async Task LoadAlertasStockAsync(int limiteStockBajo = 5)
        {
            // Obtener todos los insumos
            // ** NOTA: Asumo que el modelo de Insumo está en Negocio/Modelos
            var response = await _client.From<Insumo>().Get();
            var insumos = response.Models;

            // Contar aquellos cuya cantidad en stock es menor o igual al límite (usamos StockActual)
            AlertasDeStock = insumos
                // ** Usamos StockActual, como se ve en tu PedidoService al descontar stock **
                .Count(i => i.StockActual <= limiteStockBajo);
        }

    }
}