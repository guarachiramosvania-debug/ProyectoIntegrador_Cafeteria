using CoffeTime.Negocio.Servicios;
using System.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Threading.Tasks;
using CoffeTime.Datos.Repositorios;
using System.Globalization; // Necesario para el formato de moneda en el Code-Behind

namespace CoffeTime.Presentacion.Views
{
    public partial class ReportesView : Window
    {
        private readonly ReporteService _service = new ReporteService();
        private readonly UsuarioRepository usuarioRepo = new UsuarioRepository();

        // Variables de clase para guardar los datos cargados y poder exportarlos
        private (decimal TotalVentas, int TotalPedidos, decimal TicketPromedio) _resumen;
        private List<VentaDiariaReporteTupla> _ventasDiarias;
        private List<ProductoMasVendidoReporteTupla> _productosVendidos;
        private List<VentaMensualReporteTupla> _ventasMensuales = new List<VentaMensualReporteTupla>();

        // Definimos un CultureInfo para el formato Boliviano (Bs)
        private readonly CultureInfo bolivianCulture = new CultureInfo("es-BO");

        public ReportesView()
        {
            InitializeComponent();

            // Lógica de Permisos
            if (!PermisosService.EsAdmin())
            {
                MessageBox.Show("No tienes permisos para acceder a esta sección.");
                this.Tag = "DENIED";
                return;
            }

            // Establecer el símbolo de moneda para el contexto local (seguridad adicional)
            bolivianCulture.NumberFormat.CurrencySymbol = "Bs";

            // Inicializar el año para el reporte mensual
            txtAnioReportes.Text = DateTime.Today.Year.ToString();
            
            // Enlazar eventos (si no están ya enlazados en el XAML)
            btnExportarReporte.Click += BtnExportarReporte_Click;
            btnCargarVentasMensuales.Click += BtnCargarVentasMensuales_Click;

            // Iniciar la carga de datos y el estado Online
            CargarDatos();
            MantenerUsuarioOnline();
        }

        private async void MantenerUsuarioOnline()
        {
            // Lógica para mantener el estado de usuario online
            if (App.Current.Properties["IdUsuario"] is long id)
            {
                var usuario = await usuarioRepo.ObtenerPorIdAsync(id);

                if (usuario != null)
                {
                    usuario.Online = true;
                    await usuarioRepo.ActualizarOnlineAsync(usuario.IdUsuario, true);
                }
            }
        }

        private async void CargarDatos()
        {
            try
            {
                // Obtener el año para el reporte mensual
                if (!int.TryParse(txtAnioReportes.Text, out int anio))
                {
                    MessageBox.Show("Por favor, ingrese un año válido (ej: 2025).", "Error de Entrada", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

        

                
                var resumenTask = _service.ObtenerResumenGeneralAsync();
                var ventasDiariasTask = _service.ObtenerVentasPorDiaAsync();
                var productosVendidosTask = _service.ObtenerProductosMasVendidosAsync();
                var ventasMensualesTask = _service.ObtenerVentasMensualesAsync(anio);

                
                await Task.WhenAll(resumenTask, ventasDiariasTask, productosVendidosTask, ventasMensualesTask);

                // 1. Asignar Resumen General (Tupla)
                _resumen = resumenTask.Result;
                // Usamos el formato explícito para asegurar 'Bs' 
                txtVentasTotales.Text = $"Bs {_resumen.TotalVentas:N2}";
                txtTotalPedidos.Text = _resumen.TotalPedidos.ToString();
                txtTicketPromedio.Text = $"Bs {_resumen.TicketPromedio:N2}";

                // 2. Asignar Ventas por Día (Lista de tuplas)
                _ventasDiarias = ventasDiariasTask.Result;
                dgVentasPorDia.ItemsSource = _ventasDiarias;

                // 3. Asignar Productos Más Vendidos (Lista de tuplas)
                _productosVendidos = productosVendidosTask.Result;
                dgProductosMasVendidos.ItemsSource = _productosVendidos;

                // 4. Asignar Reporte Mensual
                _ventasMensuales = ventasMensualesTask.Result;
                dgVentasMensuales.ItemsSource = _ventasMensuales;
            }
            catch (Exception ex)
            {
                // Mostrar un mensaje de error si alguna de las tareas falla
                MessageBox.Show($"Error al cargar datos del reporte: {ex.Message}", "Error de Carga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Maneja el botón de Cargar para el reporte mensual
        private async void BtnCargarVentasMensuales_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtAnioReportes.Text, out int anio))
            {
                await CargarReporteMensual(anio);
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un año válido (ej: 2025).", "Error de Entrada", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task CargarReporteMensual(int anio)
        {
            try
            {
                _ventasMensuales = await _service.ObtenerVentasMensualesAsync(anio);
                dgVentasMensuales.ItemsSource = _ventasMensuales;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar ventas mensuales para el año {anio}: {ex.Message}", "Error de Carga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 🚀 Implementación de Exportar a TXT
        private void BtnExportarReporte_Click(object sender, RoutedEventArgs e)
        {
            // Se valida que la data esencial esté cargada antes de exportar
            if (_ventasDiarias == null || _productosVendidos == null || _ventasMensuales == null)
            {
                MessageBox.Show("Los datos del reporte no están completamente cargados. Por favor, espere o recargue los datos.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 1. Configurar y mostrar el diálogo para guardar
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Archivos de Texto (*.txt)|*.txt",
                FileName = $"Reporte_CoffeTime_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // 2. Generar el contenido del archivo TXT
                    string contenido = GenerarContenidoReporte();

                    // 3. Escribir el archivo
                    File.WriteAllText(saveFileDialog.FileName, contenido);

                    MessageBox.Show($"Reporte exportado exitosamente a:\n{saveFileDialog.FileName}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar el reporte: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        
        private string GenerarContenidoReporte()
        {
            StringBuilder sb = new StringBuilder();
            string separador = "=================================================\n";
            string linea = "-------------------------------------------------\n";

            sb.AppendLine(separador);
            sb.AppendLine($"REPORTE DE ESTADÍSTICAS - COFFE TIME");
            sb.AppendLine($"FECHA DE EXPORTACIÓN: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine(separador);
            sb.AppendLine();

            // --- RESUMEN GENERAL ---
            sb.AppendLine("--- RESUMEN GENERAL ---");
            
            sb.AppendLine($"Ventas Totales: Bs {_resumen.TotalVentas:N2}");
            sb.AppendLine($"Total de Pedidos: {_resumen.TotalPedidos}");
            sb.AppendLine($"Ticket Promedio: Bs {_resumen.TicketPromedio:N2}");
            sb.Append(linea);
            sb.AppendLine();

            // --- VENTAS POR DÍA ---
            sb.AppendLine("--- VENTAS POR DÍA ---");
            sb.AppendLine(string.Format("{0,-15} {1,-10} {2,-15}", "Fecha", "Pedidos", "Total"));
            foreach (var item in _ventasDiarias)
            {
                
                sb.AppendLine(string.Format("{0,-15} {1,-10} Bs {2,-12:N2}", item.Fecha.ToShortDateString(), item.CantidadPedidos, item.TotalVentas));
            }
            sb.Append(linea);
            sb.AppendLine();

            // --- PRODUCTOS MÁS VENDIDOS ---
            sb.AppendLine("--- PRODUCTOS MÁS VENDIDOS ---");
            sb.AppendLine(string.Format("{0,-5} {1,-30} {2,-10} {3,-15}", "Pos.", "Producto", "Vendidos", "Total"));
            foreach (var item in _productosVendidos)
            {
                
                sb.AppendLine(string.Format("{0,-5} {1,-30} {2,-10} Bs {3,-12:N2}", item.Posicion, item.NombreProducto, item.CantidadVendida, item.MontoTotal));
            }
            sb.Append(linea);
            sb.AppendLine();

            // --- VENTAS MENSUALES ---
            sb.AppendLine($"--- VENTAS MENSUALES (Año {txtAnioReportes.Text}) ---");
            sb.AppendLine(string.Format("{0,-15} {1,-10} {2,-15}", "Mes", "Pedidos", "Total"));
            foreach (var item in _ventasMensuales)
            {
                
                sb.AppendLine(string.Format("{0,-15} {1,-10} Bs {2,-12:N2}", item.MesNombre, item.CantidadPedidos, item.TotalVentas));
            }
            sb.Append(separador);

            return sb.ToString();
        }
    }
}