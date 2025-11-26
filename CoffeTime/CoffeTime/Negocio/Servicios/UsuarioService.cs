using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsuarioModel = CoffeTime.Negocio.Modelos.Usuario;

namespace CoffeTime.Negocio.Servicios
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public Task<List<UsuarioModel>> ObtenerTodosAsync()
            => _usuarioRepository.ObtenerTodosAsync();

        public Task<UsuarioModel?> AutenticarAsync(string nombreUsuario, string contrasena)
            => _usuarioRepository.ObtenerPorCredencialesAsync(nombreUsuario, contrasena);

        public Task<bool> CrearUsuarioAsync(UsuarioModel usuario)
            => _usuarioRepository.CrearUsuarioAsync(usuario);

        public Task<bool> ActualizarPerfilAsync(UsuarioModel usuario)
            => _usuarioRepository.ActualizarPerfilAsync(usuario);

        public Task<bool> ActualizarOnlineAsync(long idUsuario, bool online)
            => _usuarioRepository.ActualizarOnlineAsync(idUsuario, online);

        public Task<bool> EliminarUsuarioAsync(long idUsuario)
            => _usuarioRepository.EliminarUsuarioAsync(idUsuario);
    }
}
