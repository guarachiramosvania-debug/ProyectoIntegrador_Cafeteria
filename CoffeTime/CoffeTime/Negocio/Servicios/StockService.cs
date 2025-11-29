using CoffeTime.Datos.Repositorios;
using System.Threading.Tasks;

public class StockService
{
    private readonly DetallePedidoRepository _detalleRepo;
    private readonly ProductoInsumoRepository _prodInsRepo;
    private readonly InsumoRepository _insumoRepo;

    public StockService()
    {
        _detalleRepo = new DetallePedidoRepository();
        _prodInsRepo = new ProductoInsumoRepository();
        _insumoRepo = new InsumoRepository();
    }

    // =============================================================
    //  DESCONTAR STOCK AL PAGAR PEDIDO
    // =============================================================
    public async Task DescontarStockDesdePedido(long idPedido)
    {
        // 1) Obtener detalles del pedido
        var detalles = await _detalleRepo.ObtenerPorPedido(idPedido);
        if (detalles == null || detalles.Count == 0)
            return;

        foreach (var det in detalles)
        {
            // 2) Obtener insumos que usa este producto
            var insumosProducto = await _prodInsRepo.ObtenerPorProducto((int)det.IdProducto);

            foreach (var ip in insumosProducto)
            {
                // 3) Obtener el insumo real
                var insumo = await _insumoRepo.ObtenerPorIdAsync((int)ip.IdInsumo);
                if (insumo == null)
                    continue;

                // 4) Calcular cantidad consumida
                //    ip.Cantidad = cantidad necesaria por unidad de producto
                decimal cantidadConsumida = ip.Cantidad * det.Cantidad;

                insumo.StockActual -= cantidadConsumida;

                if (insumo.StockActual < 0)
                    insumo.StockActual = 0;

                // 5) Guardar cambios
                await _insumoRepo.ActualizarStock(insumo.IdInsumo, insumo.StockActual);
            }
        }
    }
}
