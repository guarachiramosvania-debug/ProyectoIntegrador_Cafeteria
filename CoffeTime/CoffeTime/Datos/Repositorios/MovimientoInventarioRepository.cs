using CoffeTime.Negocio.Modelos;
using Supabase; // ← necesario para usar 'Client'
using System.Threading.Tasks;

namespace CoffeTime.Datos.Repositorios
{
    public class MovimientoInventarioRepository
    {
        private readonly Client _client;

        public MovimientoInventarioRepository(Client client)
        {
            _client = client;
        }

        public async Task<bool> RegistrarMovimientoAsync(MovimientoInventario movimiento)
        {
            try
            {
                await _client
                    .From<MovimientoInventario>()
                    .Insert(movimiento);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}