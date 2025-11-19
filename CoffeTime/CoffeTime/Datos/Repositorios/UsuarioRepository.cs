using CoffeTime.Negocio.Modelos; // o ProyectoIntegrador_Cafeteria.Negocio.Modelos
using ProyectoIntegrador_Cafeteria.Negocio.Modelos;
using Supabase;
using System.Threading.Tasks;

namespace CoffeTime.Datos.Repositorios
{
    public class UsuarioRepository
    {
        private readonly Client _client;

        public UsuarioRepository(Client client)
        {
            _client = client;
        }

        public async Task<bool> CrearUsuarioAsync(Usuario usuario)
        {
            try
            {
                await _client.From<Usuario>().Insert(usuario);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Usuario?> ObtenerPorIdAsync(long idUsuario)
        {
            try
            {
                var response = await _client
                    .From<Usuario>()
                    .Where(x => x.IdUsuario == idUsuario)
                    .Single();
                return response;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            try
            {
                var response = await _client
                    .From<Usuario>()
                    .Where(x => x.NombreUsuario == nombreUsuario)
                    .Single();
                return response;
            }
            catch
            {
                return null;
            }
        }
    }
}