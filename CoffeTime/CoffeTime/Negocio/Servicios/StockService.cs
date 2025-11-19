// /Negocio/Servicios/StockService.cs
using CoffeTime.Negocio.Modelos;
using CoffeTime.Datos.Repositorios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    public class StockService
    {
        private readonly InsumoRepository _insumoRepository;

        public StockService(InsumoRepository insumoRepository)
        {
            _insumoRepository = insumoRepository;
        }

        /// <summary>
        /// Valida si hay stock suficiente para cada producto (tratado como insumo) en los detalles.
        /// </summary>
        public async Task<bool> HayStockSuficienteAsync(List<DetallePedido> detallesPedido)
        {
            foreach (var detalle in detallesPedido)
            {
                // Asumimos: detalle.IdProducto == IdInsumo
                var insumo = await _insumoRepository.ObtenerPorId(detalle.IdProducto);
                if (insumo == null || insumo.StockActual < detalle.Cantidad)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Descuenta el stock de cada insumo (producto) tras confirmar el pedido.
        /// </summary>
        public async Task<bool> DescontarStockPorPedidoAsync(List<DetallePedido> detallesPedido)
        {
            foreach (var detalle in detallesPedido)
            {
                var insumo = await _insumoRepository.ObtenerPorId(detalle.IdProducto);
                if (insumo == null || insumo.StockActual < detalle.Cantidad)
                    return false;

                var nuevoStock = insumo.StockActual - detalle.Cantidad;
                var actualizado = await _insumoRepository.ActualizarStock(detalle.IdProducto, nuevoStock);
                if (!actualizado)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Incrementa el stock de un insumo (entrada de proveedor).
        /// </summary>
        public async Task<bool> AgregarStockAsync(long insumoId, decimal cantidadAdicional)
        {
            var insumo = await _insumoRepository.ObtenerPorId(insumoId);
            if (insumo == null) return false;

            var nuevoStock = insumo.StockActual + cantidadAdicional;
            return await _insumoRepository.ActualizarStock(insumoId, nuevoStock);
        }
    }
}