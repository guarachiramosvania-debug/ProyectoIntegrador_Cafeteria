// /Datos/Repositorios/PedidoRepository.cs
using CoffeTime.Negocio.Modelos;
using Supabase;
using System.Threading.Tasks;

namespace CoffeTime.Datos.Repositorios
{
    public class PedidoRepository
    {
        private readonly Client _client;

        public PedidoRepository(Client client)
        {
            _client = client;
        }

        /// <summary>
        /// Crea un nuevo detalle de pedido en la tabla 'detalle_pedido'
        /// </summary>
        public async Task<bool> CrearDetalleAsync(DetallePedido detalle)
        {
            try
            {
                await _client
                    .From<DetallePedido>()
                    .Insert(detalle);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Actualiza el estado de un pedido (campo 'estado') por su ID
        /// </summary>
        public async Task<bool> ActualizarEstadoAsync(long idPedido, string nuevoEstado)
        {
            try
            {
                var pedidoParcial = new Pedido
                {
                    IdPedido = idPedido,
                    Estado = nuevoEstado
                };

                await _client
                    .From<Pedido>()
                    .Where(x => x.IdPedido == idPedido)
                    .Update(pedidoParcial);

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Si ya tienes CrearPedidoAsync, mantenlo. Si no, aquí va también:
        public async Task<long> CrearPedidoAsync(Pedido pedido)
        {
            try
            {
                var response = await _client.From<Pedido>().Insert(pedido);
                return response.Models.Count > 0 ? response.Models[0].IdPedido : -1;
            }
            catch
            {
                return -1;
            }
        }
    }
}