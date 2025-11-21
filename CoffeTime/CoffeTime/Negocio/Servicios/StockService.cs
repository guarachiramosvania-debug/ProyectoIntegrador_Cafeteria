using CoffeTime.Negocio.Modelos;
using CoffeTime.Datos.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    public class StockService
    {
        private readonly InsumoRepository _insumoRepo;
        private readonly MovimientoInventarioRepository _movRepo;
        private readonly ProductoInsumoRepository _prodInsumoRepo;

        public StockService(InsumoRepository insumoRepo,
                            MovimientoInventarioRepository movRepo,
                            ProductoInsumoRepository prodInsumoRepo)
        {
            _insumoRepo = insumoRepo;
            _movRepo = movRepo;
            _prodInsumoRepo = prodInsumoRepo;
        }

        // ===========================================================================
        // ?? Validar stock suficiente
        // ===========================================================================
        public async Task<bool> HayStockParaProductoAsync(long idProducto)
        {
            var insumos = await _prodInsumoRepo.ObtenerPorProductoAsync(idProducto);

            foreach (var pi in insumos)
            {
                var insumo = await _insumoRepo.ObtenerPorId(pi.IdInsumo);

                if (insumo == null) return false;
                if (insumo.StockActual < pi.Cantidad) return false;
            }

            return true;
        }

        // ===========================================================================
        // ?? Descontar insumos tras una venta
        // ===========================================================================
        public async Task<bool> DescontarStockProductoAsync(long idProducto, long idUsuario)
        {
            var insumos = await _prodInsumoRepo.ObtenerPorProductoAsync(idProducto);

            foreach (var pi in insumos)
            {
                var insumo = await _insumoRepo.ObtenerPorId(pi.IdInsumo);
                if (insumo == null) return false;

                var nuevoStock = insumo.StockActual - pi.Cantidad;

                await _insumoRepo.ActualizarStock(insumo.IdInsumo, nuevoStock);

                // registrar movimiento
                await _movRepo.RegistrarMovimientoAsync(new MovimientoInventario
                {
                    IdInsumo = pi.IdInsumo,
                    TipoMovimiento = "salida",
                    Cantidad = pi.Cantidad,
                    Fecha = DateTime.Now,
                    UsuarioResponsable = idUsuario,
                    CostoTotal = 0
                });
            }

            return true;
        }
    }
}
