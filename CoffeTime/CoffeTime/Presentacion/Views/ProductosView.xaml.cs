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
    // Define una clase simple para representar un Producto (simulación)
    public class Producto
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public string Descripcion { get; set; }
    }

    /// <summary>
    /// Lógica de interacción para ProductosView.xaml
    /// </summary>
    public partial class ProductosView : Window
    {
        public ProductosView()
        {
            try
            {
                // Inicializa los componentes definidos en ProductosView.xaml
                InitializeComponent();

                // Llama al método de carga inicial de datos
                CargarProductosIniciales();

                // Nota: En un proyecto real, se asignaría un ViewModel aquí (this.DataContext = new ProductosViewModel();)
            }
            catch (Exception ex)
            {
                // Captura cualquier error crítico durante la inicialización de la ventana
                MessageBox.Show($"Error al inicializar la vista de Productos: {ex.Message}", "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
                // Si el error es irrecuperable, puedes considerar cerrar la aplicación o la vista
                // this.Close(); 
            }
        }

        /// <summary>
        /// Simula la carga de datos desde una fuente (DB, API, etc.) con manejo de errores.
        /// </summary>
        private void CargarProductosIniciales()
        {
            try
            {
                // --- SIMULACIÓN DE LÓGICA DE NEGOCIO ---
                var listaDeProductos = new List<Producto>
                {
                    new Producto { Nombre = "Espresso", Precio = 2.50M, Descripcion = "Intenso y aromático" },
                    new Producto { Nombre = "Cappuccino", Precio = 3.50M, Descripcion = "Con espuma de leche cremosa" },
                    // Agrega más productos aquí...
                };

                // Suponiendo que tienes un ListBox o ItemsControl en el XAML llamado 'ListaProductosControl'
                // ListaProductosControl.ItemsSource = listaDeProductos;

                // Si no tienes un control de lista, solo mostramos un mensaje de éxito
                Console.WriteLine("Productos cargados exitosamente.");
            }
            catch (System.Data.Common.DbException dbEx)
            {
                // Captura errores específicos de la base de datos
                MessageBox.Show($"Error de base de datos al cargar productos: {dbEx.Message}", "Error de BD", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Captura cualquier otro error general
                MessageBox.Show($"Ocurrió un error inesperado al cargar los productos: {ex.Message}", "Error de Carga", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Manejador de evento para el botón "Nuevo Producto" (simulado).
        /// Aquí se simula la validación de un formulario.
        /// </summary>
        /// <param name="sender">El objeto que levantó el evento (el botón).</param>
        /// <param name="e">Datos del evento.</param>
        private void NuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            // Nota: Este método requeriría que el botón en XAML tenga Command="{Binding AddProductCommand}" cambiado a Click="NuevoProducto_Click"

            // --- SIMULACIÓN DE DATOS DEL FORMULARIO ---
            string nombreProducto = "Nuevo Café"; // Asumimos que obtienes esto de un TextBox
            decimal precio = 0M; // Inicializamos el precio

            try
            {
                // 1. Validar el nombre
                if (string.IsNullOrWhiteSpace(nombreProducto))
                {
                    throw new ArgumentException("El nombre del producto no puede estar vacío.");
                }

                // 2. Validar y convertir el precio (simulación de Try-Parse)
                string precioTexto = "4.99"; // Asumimos que esto viene de un TextBox

                if (!decimal.TryParse(precioTexto, out precio) || precio <= 0)
                {
                    throw new FormatException("El precio debe ser un número positivo y válido.");
                }

                // 3. Ejecutar la lógica de guardado
                GuardarProducto(nombreProducto, precio);

            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Validación de Datos", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (FormatException fEx)
            {
                MessageBox.Show(fEx.Message, "Validación de Precio", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al intentar registrar el producto: {ex.Message}", "Error de Registro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Simula la lógica de guardar el producto en el sistema.
        /// </summary>
        private void GuardarProducto(string nombre, decimal precio)
        {
            // Lógica para enviar datos a la capa de negocio o base de datos.
            MessageBox.Show($"Producto '{nombre}' (${precio}) registrado con éxito.", "Registro Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
            // CargarProductosIniciales(); // Opcional: Recargar la lista después de guardar
        }

        // Puedes agregar más manejadores de eventos (Ej: al seleccionar un producto, al hacer clic en editar, etc.)
    }
}
