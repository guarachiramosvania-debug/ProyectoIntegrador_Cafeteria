using ProyectoIntegrador_Cafeteria.Negocio.Modelos; // ? Solo este using
using Supabase;
using System.Collections.Generic; // ?? ¡Importante!
using System.Threading.Tasks;

namespace CoffeTime.Datos.Repositorios
{
    public class UsuarioRepository
    {
        private readonly Client _client;

        public UsuarioRepository()
        {
        }

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

        // ?????? AÑADE ESTE MÉTODO AQUÍ ??????
        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            try
            {
                var response = await _client.From<Usuario>().Get();
                return response.Models; // ? Correcto
            }
            catch
            {
                return new List<Usuario>();
            }
        }

        internal async Task GetAll()
        {
            throw new NotImplementedException();
        }
    }
}