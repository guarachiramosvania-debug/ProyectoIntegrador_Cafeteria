using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _repo;

        public UsuarioService(UsuarioRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Usuario>> ObtenerTodosAsync()
        {
            return _repo.ObtenerTodosAsync();
        }

        public async Task<bool> CrearUsuarioAsync(Usuario usuario)
        {
            return await _repo.CrearUsuarioAsync(usuario);
        }

        public async Task<bool> EliminarUsuarioAsync(long idUsuario)
        {
            return await _repo.EliminarUsuarioAsync(idUsuario);
        }

        public async Task<Usuario?> AutenticarAsync(string usuario, string contrasena)
        {
            return await _repo.ObtenerPorCredencialesAsync(usuario, contrasena);
        }
    }
}
