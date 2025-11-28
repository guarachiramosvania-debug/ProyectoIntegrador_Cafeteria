using CoffeTime.Negocio.Modelos; // Ajusta este namespace si es necesario
using CoffeTime.Presentacion.Commands; // Necesario para RelayCommand
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CoffeTime.Presentacion.Views;

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
            MantenerUsuarioOnline();

            // Asignar el ViewModel
            DataContext = new DashboardViewModel();

            var vm = (DashboardViewModel)DataContext;

            // Ejecutar el comando de carga de datos al iniciar
            if (vm.LoadDashboardDataCommand.CanExecute(null))
            {
                vm.LoadDashboardDataCommand.Execute(null);
            }
        }

        // Lógica de simulación de estado de usuario
        private async void MantenerUsuarioOnline()
        {
            // Nota: Este bloque usa clases que deben estar definidas en tu proyecto 
            // (UsuarioRepository y App.Current.Properties)
            if (App.Current.Properties["IdUsuario"] is long id)
            {
                var usuario = await new UsuarioRepository().ObtenerPorIdAsync(id);

                if (usuario != null)
                {
                    await new UsuarioRepository().ActualizarOnlineSoloAsync(usuario.IdUsuario, true);
                }
            }
        }

        // 🎯 1. NAVEGACIÓN A PEDIDOS (Acceso Rápido y Nuevo Pedido Rápido)
        private void AbrirPedidosView()
        {
            try
            {
                // Instanciar la ventana de Pedidos
                PedidosView pedidosWindow = new PedidosView();

                // Mostrar la nueva ventana
                pedidosWindow.Show();

                // Cerrar la ventana actual (Dashboard)
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ventana de Pedidos: " + ex.Message, "Error de Navegación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Manejador del botón de Acceso Rápido a Pedidos
        private void btnAccesoPedidos_Click(object sender, RoutedEventArgs e)
        {
            AbrirPedidosView();
        }

        // Manejador del botón Nuevo Pedido Rápido
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
    // BASE DE VIEWMODEL (Para implementar INotifyPropertyChanged)
    // =========================================================
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    // =========================================================
    // VIEWMODEL ESPECÍFICO DEL DASHBOARD
    // =========================================================
    public class DashboardViewModel : ViewModelBase
    {
        // -----------------------------------------------------
        // Propiedades de Datos (Dashboard Cards)
        // -----------------------------------------------------
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

        private int _pedidosDelDia;
        public int PedidosDelDia
        {
            get => _pedidosDelDia;
            set
            {
                _pedidosDelDia = value;
                OnPropertyChanged();
            }
        }

        private int _alertasDeStock;
        public int AlertasDeStock
        {
            get => _alertasDeStock;
            set
            {
                _alertasDeStock = value;
                OnPropertyChanged();
            }
        }

        // -----------------------------------------------------
        // Comandos (Bindings)
        // -----------------------------------------------------
        public ICommand LoadDashboardDataCommand { get; private set; }
        public ICommand NewQuickOrderCommand { get; private set; }
        public ICommand NavigateToOrdersCommand { get; private set; }
        public ICommand NavigateToInventoryCommand { get; private set; }

        public DashboardViewModel()
        {
            // Inicialización de comandos
            LoadDashboardDataCommand = new RelayCommand(ExecuteLoadDashboardData);

            // Los comandos de navegación se mantienen para los bindings de XAML, 
            // pero la lógica real la maneja el Code-Behind (eventos Click)
            NavigateToOrdersCommand = new RelayCommand(p => { /* Lógica en Code-Behind */ });
            NavigateToInventoryCommand = new RelayCommand(p => { /* Lógica en Code-Behind */ });
            NewQuickOrderCommand = new RelayCommand(p => { /* Lógica en Code-Behind */ });
        }


        private void ExecuteLoadDashboardData(object parameter)
        {
            try
            {
                // SIMULACIÓN DE CARGA DE DATOS
                VentasDelDia = 450.75m;
                PedidosDelDia = 12;
                AlertasDeStock = 3;
            }
            catch { /* Manejo de errores */ }
        }

    }
}