using CoffeTime.Datos.Conexion;
using CoffeTime.Negocio.Modelos;
using Supabase;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeTime.Datos.Repositorios
{
    public class DetallePedidoRepository
    {
        private readonly Client _db;

        public DetallePedidoRepository()
        {
            _db = SupabaseContext.Client;
        }

        public async Task<List<DetallePedido>> ObtenerPorPedido(long idPedido)
        {
            var resp = await _db
                .From<DetallePedido>()
                .Where(d => d.IdPedido == idPedido)
                .Get();

            return resp.Models;
        }
    }
}
