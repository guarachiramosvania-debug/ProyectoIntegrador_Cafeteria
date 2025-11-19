using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CoffeTime.Presentacion.Views
{
    /// <summary>
    /// Interaction logic for ProveedoresView.xaml
    /// </summary>
    public partial class ProveedoresView : UserControl
    {
        public ProveedoresView()
        {
            InitializeComponent();

            ListaProveedores.ItemsSource = new List<dynamic>
        {
            new { Nombre="Café Premium S.A.", ContactoNombre="Juan Pérez", Email="ventas@cafepremium.com", Telefono="+1234567890", FechaRegistro="31/12/2024" },
            new { Nombre="Lácteos del Valle", ContactoNombre="Ana García", Email="pedidos@lacteosvalle.com", Telefono="+1234567891", FechaRegistro="31/12/2024" },
            new { Nombre="Dulces y Postres", ContactoNombre="Roberto Sánchez", Email="contacto@dulcesypostres.com", Telefono="+1234567892", FechaRegistro="31/12/2024" }
        };
        }
    }
}