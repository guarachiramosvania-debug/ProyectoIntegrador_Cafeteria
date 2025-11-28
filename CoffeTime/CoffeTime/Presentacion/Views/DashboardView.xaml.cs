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
            MantenerUsuarioOnline();
            DataContext = new DashboardViewModel();

            var vm = (DashboardViewModel)DataContext;
            vm.LoadDashboardDataCommand.Execute(null);
        }
        private async void MantenerUsuarioOnline()
        {
            if (App.Current.Properties["IdUsuario"] is long id)
            {
                var usuario = await new UsuarioRepository().ObtenerPorIdAsync(id);

                if (usuario != null)
                {
                    await new UsuarioRepository().ActualizarOnlineSoloAsync(usuario.IdUsuario, true);
                }
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
        public decimal VentasDelDia { get; set; }
        public int PedidosDelDia { get; set; }
        public int AlertasDeStock { get; set; }

        public ICommand LoadDashboardDataCommand { get; private set; }

        public DashboardViewModel()
        {
            LoadDashboardDataCommand = new RelayCommand(ExecuteLoadDashboardData);
        }
       

        private void ExecuteLoadDashboardData(object parameter)
        {
            try
            {
                VentasDelDia = 450.75m;
                PedidosDelDia = 12;
                AlertasDeStock = 3;

                OnPropertyChanged(nameof(VentasDelDia));
                OnPropertyChanged(nameof(PedidosDelDia));
                OnPropertyChanged(nameof(AlertasDeStock));
            }
            catch { }
        }

    }
}
