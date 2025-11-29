using CoffeTime.Datos.Conexion;
using CoffeTime.Negocio.Modelos;
using Supabase;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProductoInsumoRepository
{
    private readonly Client _client;

    public ProductoInsumoRepository()
    {
        _client = SupabaseContext.Client;
    }

    public async Task<List<ProductoInsumo>> ObtenerPorProductoAsync(long idProducto)
    {
        var resp = await _client
            .From<ProductoInsumo>()
            .Where(pi => pi.IdProducto == idProducto)
            .Get();

        return resp.Models;
    }

    public async Task<bool> Insert(ProductoInsumo pi)
    {
        var result = await _client
            .From<ProductoInsumo>()
            .Insert(pi);

        return result.Models.Count > 0;
    }

    public async Task<List<ProductoInsumo>> ObtenerPorProducto(int idProducto)
    {
        var resp = await _client
            .From<ProductoInsumo>()
            .Where(pi => pi.IdProducto == idProducto)
            .Get();

        return resp.Models;
    }
    // Para obtener cuántas unidades de insumo usa un producto
    public async Task<int> CantidadNecesariaAsync(int idProducto, int idInsumo)
    {
        var resp = await _client
            .From<ProductoInsumo>()
            .Where(pi => pi.IdProducto == idProducto && pi.IdInsumo == idInsumo)
            .Get();

        var dato = resp.Models.FirstOrDefault();
        return (int)(dato?.Cantidad ?? 0);
    }

}
