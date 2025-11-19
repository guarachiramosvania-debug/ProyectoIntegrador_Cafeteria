using CoffeTime.Negocio.Modelos;
using Supabase;


public class InsumoRepository
{
    private readonly Client _client;

    public InsumoRepository(Client client)
    {
        _client = client;
    }

    /// <summary>
    /// Obtiene un insumo por su ID (usa el campo IdInsumo como clave primaria)
    /// </summary>
    public async Task<Insumo?> ObtenerPorId(long idInsumo)
    {
        try
        {
            var response = await _client
                .From<Insumo>()
                .Where(x => x.IdInsumo == idInsumo)
                .Single();

            return response;
        }
        catch
        {
            // Si no encuentra o hay error, devuelve null
            return null;
        }
    }

    /// <summary>
    /// Actualiza el stock_actual de un insumo
    /// </summary>
    public async Task<bool> ActualizarStock(long idInsumo, decimal nuevoStock)
    {
        var insumo = await ObtenerPorId(idInsumo);
        if (insumo == null) return false;

        // Solo actualizamos el campo stock_actual
        insumo.StockActual = nuevoStock;

        try
        {
            await _client
                .From<Insumo>()
                .Where(x => x.IdInsumo == idInsumo)
                .Update(insumo);
            return true;
        }
        catch
        {
            return false;
        }
    }
}