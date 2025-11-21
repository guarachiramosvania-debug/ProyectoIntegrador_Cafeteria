using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace CoffeTime.Presentacion.ViewModels
{
    // Clase base para implementar INotifyPropertyChanged
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
        // ======================================
        // PROPIEDADES (Indicadores del Dashboard)
        // ======================================
        private decimal _ventasDelDia;
        public decimal VentasDelDia
        {
            get => _ventasDelDia;
            set
            {
                _ventasDelDia = value;
                OnPropertyChanged();
            }
        }
        // ... (Otras propiedades de PedidosDelDia y AlertasDeStock, como antes)

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
            set
            {
                _alertasDeStock = value;
                OnPropertyChanged();
                if (value > 0)
                {
                    // Usar MessageBox.Show directamente si se permite en el VM.
                    // En MVVM estricto, esto usaría un servicio.
                    MessageBox.Show($"¡ATENCIÓN! Hay {value} insumos con stock bajo.", "Alerta de Stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        // ======================================
        // COMANDOS
        // ======================================
        public ICommand LoadDashboardDataCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand NewQuickOrderCommand { get; private set; }
        public ICommand NavigateToUsersCommand { get; private set; }
        public ICommand NavigateToProductsCommand { get; private set; }
        public ICommand NavigateToOrdersCommand { get; private set; }
        public ICommand NavigateToInventoryCommand { get; private set; }
        public ICommand NavigateToSuppliersCommand { get; private set; }
        public ICommand NavigateToReportsCommand { get; private set; }


        public DashboardViewModel()
        {
            // Inicialización de comandos usando RelayCommand (ahora accesible)
            LoadDashboardDataCommand = new RelayCommand(ExecuteLoadDashboardData, CanExecuteLoadDashboardData);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            NewQuickOrderCommand = new RelayCommand(ExecuteNewQuickOrder);

            NavigateToUsersCommand = new RelayCommand(ExecuteNavigateToUsers);
            NavigateToProductsCommand = new RelayCommand(ExecuteNavigateToProducts);
            NavigateToOrdersCommand = new RelayCommand(ExecuteNavigateToOrders);
            NavigateToInventoryCommand = new RelayCommand(ExecuteNavigateToInventory);
            NavigateToSuppliersCommand = new RelayCommand(ExecuteNavigateToSuppliers);
            NavigateToReportsCommand = new RelayCommand(ExecuteNavigateToReports);
        }

        // ======================================
        // MÉTODOS DE LÓGICA Y NAVEGACIÓN
        // ======================================

        private bool CanExecuteLoadDashboardData(object parameter) => true;

        private void ExecuteLoadDashboardData(object parameter)
        {
            try
            {
                // Lógica de validación (ej. permisos)
                // if (!User.HasPermission("DashboardRead")) throw new UnauthorizedAccessException("Permisos insuficientes.");

                // Lógica de obtención de datos
                VentasDelDia = GetVentasHoy();
                PedidosDelDia = GetPedidosHoy();
                AlertasDeStock = GetAlertasStock();
            }
            catch (Exception ex)
            {
                // Manejo de errores (try-catch)
                MessageBox.Show($"Error al cargar el resumen: {ex.Message}", "Error de Carga", MessageBoxButton.OK, MessageBoxImage.Error);
                VentasDelDia = 0;
                PedidosDelDia = 0;
                AlertasDeStock = 0;
            }
        }

        // Métodos de simulación (reemplazar con llamadas a servicios)
        private decimal GetVentasHoy() => 450.75m;
        private int GetPedidosHoy() => 12;
        private int GetAlertasStock() => 3;

        // Métodos de navegación (Ejemplo con MessageBox.Show)
        private void ExecuteLogout(object parameter) => MessageBox.Show("Cerrando sesión...", "Acción");
        private void ExecuteNewQuickOrder(object parameter) => MessageBox.Show("Abriendo formulario de Pedido Rápido.", "Acción");
        private void ExecuteNavigateToUsers(object parameter) => MessageBox.Show("Navegando a la Gestión de Usuarios.", "Navegación");
        private void ExecuteNavigateToProducts(object parameter) => MessageBox.Show("Navegando a la Gestión de Productos.", "Navegación");
        private void ExecuteNavigateToOrders(object parameter) => MessageBox.Show("Navegando a la Gestión de Pedidos.", "Navegación");
        private void ExecuteNavigateToInventory(object parameter) => MessageBox.Show("Navegando a la Gestión de Inventario.", "Navegación");
        private void ExecuteNavigateToSuppliers(object parameter) => MessageBox.Show("Navegando a la Gestión de Proveedores.", "Navegación");
        private void ExecuteNavigateToReports(object parameter) => MessageBox.Show("Navegando a la Gestión de Reportes.", "Navegación");
    }
}