using ProyectoIntegrador_Cafeteria.Negocio.Modelos;
using Supabase;

public class PedidoRepository
{
    private readonly Supabase.Client _client;

    public PedidoRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<long> CrearPedido(Pedido pedido)
    {
        var result = await _client
            .From<Pedido>()
            .Insert(pedido);

        return result.Models[0].IdPedido;
    }

    public async Task<Pedido> ObtenerPedido(long idPedido)
    {
        var result = await _client
            .From<Pedido>()
            .Where(p => p.IdPedido == idPedido)
            .Single();

        return result;
    }

    public async Task<List<Pedido>> ObtenerTodos()
    {
        var result = await _client
            .From<Pedido>()
            .Get();

        return result.Models;
    }

    // LA FUNCIÓN 100% FUNCIONAL
    public async Task ActualizarEstado(long idPedido, string nuevoEstado)
    {
        var pedido = await _client
            .From<Pedido>()
            .Where(p => p.IdPedido == idPedido)
            .Single();

        if (pedido == null)
            throw new Exception("El pedido no existe.");

        pedido.Estado = nuevoEstado;

        await _client
            .From<Pedido>()
            .Where(p => p.IdPedido == idPedido)
            .Update(pedido);
    }
}
