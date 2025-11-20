// InventarioService.cs
using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    public class InventarioService
    {
        private readonly MovimientoInventarioRepository _repo;

        public InventarioService(MovimientoInventarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<MovimientoInventario>> ObtenerHistorialEntradasAsync()
        {
            return await _repo.ObtenerHistorialAsync();
        }

        // Simulamos inventario actual (hasta que tengas tabla 'insumos')
        public async Task<List<object>> ObtenerInventarioActualAsync()
        {
            await Task.Delay(10); // Simular carga
            return new List<object>
            {
                new { Nombre = "Café Molido", Stock = "5000 g", Minimo = "1000 g", Estado = "OK" },
                new { Nombre = "Leche", Stock = "8000 ml", Minimo = "2000 ml", Estado = "OK" },
                new { Nombre = "Chocolate", Stock = "3000 g", Minimo = "500 g", Estado = "OK" }
            };
        }
    }
}