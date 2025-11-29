using CoffeTime.Datos.Conexion;
using CoffeTime.Negocio.Modelos;
using Supabase;

public class InsumoRepository
{
    private readonly Client _client;

    public InsumoRepository()
    {
        _client = SupabaseContext.Client; // <-- USAR CLIENTE GLOBAL FUNCIONAL
    }

    public async Task<List<Insumo>> ObtenerTodosAsync()
    {
        try
        {
            var result = await _client.From<Insumo>().Get();
            return result.Models;
        }
        catch
        {
            return new List<Insumo>();
        }
    }

    public async Task<Insumo?> ObtenerPorId(long id)
    {
        try
        {
            return await _client.From<Insumo>()
                               .Where(x => x.IdInsumo == id)
                               .Single();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ActualizarStock(long id, decimal stock)
    {
        var ins = await ObtenerPorId(id);

        if (ins == null) return false;

        ins.StockActual = stock;

        try
        {
            await _client.From<Insumo>()
                         .Where(x => x.IdInsumo == id)
                         .Update(ins);

            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<Insumo?> ObtenerPorIdAsync(int id)
    {
        var resp = await _client
            .From<Insumo>()
            .Where(x => x.IdInsumo == id)
            .Single();

        return resp;
    }

}
