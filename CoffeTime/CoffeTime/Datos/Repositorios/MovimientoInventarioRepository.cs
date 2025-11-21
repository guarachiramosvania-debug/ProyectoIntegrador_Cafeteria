using CoffeTime.Negocio.Modelos;
using Supabase;
using CoffeTime.Datos.Conexion;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeTime.Datos.Repositorios
{
    public class MovimientoInventarioRepository
    {
        private readonly Client _client;

        public MovimientoInventarioRepository()
        {
            _client = SupabaseContext.Client;
        }

        public MovimientoInventarioRepository(Client client)
        {
            _client = client;
        }

        public async Task<bool> RegistrarMovimientoAsync(MovimientoInventario movimiento)
        {
            try
            {
                await _client.From<MovimientoInventario>().Insert(movimiento);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<MovimientoInventario>> ObtenerHistorialAsync()
        {
            try
            {
                var response = await _client.From<MovimientoInventario>().Get();
                return response.Models;
            }
            catch
            {
                return new List<MovimientoInventario>();
            }
        }
    }
}
