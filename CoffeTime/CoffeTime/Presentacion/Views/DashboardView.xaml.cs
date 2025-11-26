using CoffeTime.Negocio.Modelos;
using CoffeTime.Presentacion.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CoffeTime.Presentacion.Views
{
    public partial class DashboardView : Window
    {
        public DashboardView()
        {
            InitializeComponent();

            // 🔥 Asignar el ViewModel interno como DataContext
            DataContext = new DashboardViewModel();

            // 🔥 Cargar datos al abrir
            var vm = (DashboardViewModel)DataContext;
            vm.LoadDashboardDataCommand.Execute(null);

        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CerrarSesionAutomatica();
        }
        public async void CerrarSesionAutomatica()
        {
            try
            {
                if (App.Current.Properties["IdUsuario"] == null)
                    return;

                long id = (long)App.Current.Properties["IdUsuario"];

                var repo = new UsuarioRepository();
                var user = await repo.ObtenerPorIdAsync(id);

                if (user != null)
                {
                    user.Online = false;
                    user.UltimoLogin = DateTime.Now; // opcional
                    await repo.ActualizarOnlineAsync(user.IdUsuario, true);
                }
            }
            catch { /* ignorar errores */ }
        }
    }

    // ---------------------------------------------------------
    //  AQUI ESTÁ TU VIEWMODEL (lo dejamos dentro del mismo archivo)
    // ---------------------------------------------------------

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
        // PROPIEDADES
        // ======================================

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
            LoadDashboardDataCommand = new RelayCommand(ExecuteLoadDashboardData);
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
        // LÓGICA
        // ======================================

        private void ExecuteLoadDashboardData(object parameter)
        {
            try
            {
                VentasDelDia = 450.75m;
                PedidosDelDia = 12;
                AlertasDeStock = 3;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el resumen: {ex.Message}",
                    "Error de Carga", MessageBoxButton.OK, MessageBoxImage.Error);

                VentasDelDia = 0;
                PedidosDelDia = 0;
                AlertasDeStock = 0;
            }
        }

        // ======================================
        // ACCIONES Y NAVEGACIÓN
        // ======================================

        private void ExecuteLogout(object parameter)
            => MessageBox.Show("Cerrando sesión...");

        private void ExecuteNewQuickOrder(object parameter)
            => MessageBox.Show("Abriendo formulario de Pedido Rápido.");

        private void ExecuteNavigateToUsers(object parameter)
            => MessageBox.Show("Navegando a la Gestión de Usuarios.");

        private void ExecuteNavigateToProducts(object parameter)
            => MessageBox.Show("Navegando a la Gestión de Productos.");

        private void ExecuteNavigateToOrders(object parameter)
            => MessageBox.Show("Navegando a la Gestión de Pedidos.");

        private void ExecuteNavigateToInventory(object parameter)
            => MessageBox.Show("Navegando a la Gestión de Inventario.");

        private void ExecuteNavigateToSuppliers(object parameter)
            => MessageBox.Show("Navegando a la Gestión de Proveedores.");

        private void ExecuteNavigateToReports(object parameter)
            => MessageBox.Show("Navegando a la Gestión de Reportes.");
    }
}
