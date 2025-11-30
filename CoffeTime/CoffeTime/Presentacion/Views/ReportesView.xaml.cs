using CoffeTime.Negocio.Servicios;
using System.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Threading.Tasks;
// Asegúrate de que tu Repositorio de Usuario sea accesible (asumo que está en Datos.Repositorios)
using CoffeTime.Datos.Repositorios;

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


        public ReportesView()
        {
            InitializeComponent();
            if (!PermisosService.EsAdmin())
            {
                MessageBox.Show("No tienes permisos para acceder a esta sección.");
                this.Tag = "DENIED"; // marcar que NO debe abrirse
                return;
            }

            // Enlazar eventos (si no están ya enlazados en el XAML)
            btnExportarReporte.Click += BtnExportarReporte_Click;
            btnCargarVentasMensuales.Click += BtnCargarVentasMensuales_Click;

            // Inicializar el año para el reporte mensual
            txtAnioReportes.Text = DateTime.Today.Year.ToString();

            CargarDatos();
            MantenerUsuarioOnline();
        }

        private async void MantenerUsuarioOnline()
        {
            // Lógica para mantener el estado de usuario online (se mantiene como la definiste)
            if (App.Current.Properties["IdUsuario"] is long id)
            {
                var usuario = await usuarioRepo.ObtenerPorIdAsync(id);

                if (usuario != null)
                {
                    usuario.Online = true;
                    // Asegúrate de que tu método de repositorio acepta el long y el bool
                    await usuarioRepo.ActualizarOnlineAsync(usuario.IdUsuario, true);
                }
            }
        }

        private async void CargarDatos()
        {
            try
            {
                // 1. Cargar Resumen General (Tupla)
                _resumen = await _service.ObtenerResumenGeneralAsync();
                txtVentasTotales.Text = $"{_resumen.TotalVentas:C2}";
                txtTotalPedidos.Text = _resumen.TotalPedidos.ToString();
                txtTicketPromedio.Text = $"{_resumen.TicketPromedio:C2}";

                // 2. Cargar Ventas por Día (Lista de tuplas)
                _ventasDiarias = await _service.ObtenerVentasPorDiaAsync();
                dgVentasPorDia.ItemsSource = _ventasDiarias;

                // 3. Cargar Productos Más Vendidos (Lista de tuplas)
                _productosVendidos = await _service.ObtenerProductosMasVendidosAsync();
                dgProductosMasVendidos.ItemsSource = _productosVendidos;

                // 4. Cargar Reporte Mensual (Inicialmente con el año actual)
                if (int.TryParse(txtAnioReportes.Text, out int anio))
                {
                    await CargarReporteMensual(anio);
                }
            }
            catch (Exception ex)
            {
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
                // Usamos el servicio modificado que acepta el año
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

        // Método auxiliar para formatear los datos en el TXT
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
            // Usamos las variables de clase para asegurar el formato correcto
            sb.AppendLine($"Ventas Totales: {_resumen.TotalVentas:C2}");
            sb.AppendLine($"Total de Pedidos: {_resumen.TotalPedidos}");
            sb.AppendLine($"Ticket Promedio: {_resumen.TicketPromedio:C2}");
            sb.Append(linea);
            sb.AppendLine();

            // --- VENTAS POR DÍA ---
            sb.AppendLine("--- VENTAS POR DÍA ---");
            sb.AppendLine(string.Format("{0,-15} {1,-10} {2,-15}", "Fecha", "Pedidos", "Total"));
            foreach (var item in _ventasDiarias)
            {
                // Usar :N2 para formato numérico de dos decimales para la exportación simple
                sb.AppendLine(string.Format("{0,-15} {1,-10} {2,-15:N2}", item.Fecha.ToShortDateString(), item.CantidadPedidos, item.TotalVentas));
            }
            sb.Append(linea);
            sb.AppendLine();

            // --- PRODUCTOS MÁS VENDIDOS ---
            sb.AppendLine("--- PRODUCTOS MÁS VENDIDOS ---");
            sb.AppendLine(string.Format("{0,-5} {1,-30} {2,-10} {3,-15}", "Pos.", "Producto", "Vendidos", "Total"));
            foreach (var item in _productosVendidos)
            {
                sb.AppendLine(string.Format("{0,-5} {1,-30} {2,-10} {3,-15:N2}", item.Posicion, item.NombreProducto, item.CantidadVendida, item.MontoTotal));
            }
            sb.Append(linea);
            sb.AppendLine();

            // --- VENTAS MENSUALES ---
            sb.AppendLine($"--- VENTAS MENSUALES (Año {txtAnioReportes.Text}) ---");
            sb.AppendLine(string.Format("{0,-15} {1,-10} {2,-15}", "Mes", "Pedidos", "Total"));
            foreach (var item in _ventasMensuales)
            {
                sb.AppendLine(string.Format("{0,-15} {1,-10} {2,-15:N2}", item.MesNombre, item.CantidadPedidos, item.TotalVentas));
            }
            sb.Append(separador);

            return sb.ToString();
        }
    }
}