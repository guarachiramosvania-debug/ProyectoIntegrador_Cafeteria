using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    public class PedidoService
    {
        private readonly PedidoRepository _repo = new PedidoRepository();

        public Task<List<PedidoVistaDto>> ObtenerPedidosAsync()
            => _repo.ObtenerPedidosAsync();

        public Task<bool> CancelarPedidoAsync(long idPedido)
            => _repo.CancelarPedidoAsync(idPedido);
    }
}
