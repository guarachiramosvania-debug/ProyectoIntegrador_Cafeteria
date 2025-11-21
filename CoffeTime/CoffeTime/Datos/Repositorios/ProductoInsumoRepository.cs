using CoffeTime.Negocio.Modelos;
using Supabase;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProductoInsumoRepository
{
    private readonly Client _client;

    public ProductoInsumoRepository(Client client)
    {
        _client = client;
    }

    public async Task<List<ProductoInsumo>> ObtenerPorProductoAsync(long idProducto)
    {
        var resp = await _client
            .From<ProductoInsumo>()
            .Where(pi => pi.IdProducto == idProducto)
            .Get();

        return resp.Models;
    }
}
