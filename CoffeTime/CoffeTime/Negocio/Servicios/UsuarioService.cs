using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeTime.Negocio.Servicios
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _usuarioRepository.ObtenerTodosAsync();
        }

        public async Task<(bool exito, string mensaje)> CrearUsuarioAsync(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario) ||
                string.IsNullOrWhiteSpace(usuario.Contrasena))
                return (false, "Usuario o contraseña no válidos");

            var existe = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(usuario.NombreUsuario);
            if (existe != null)
                return (false, "Nombre de usuario ya existe");

            var creado = await _usuarioRepository.CrearUsuarioAsync(usuario);
            return (creado, creado ? "Usuario creado exitosamente." : "Error al crear usuario.");
        }

        // ? LOGIN REAL: usa ObtenerPorCredencialesAsync
        public async Task<Usuario?> AutenticarAsync(string nombreUsuario, string contrasena)
        {
            return await _usuarioRepository.ObtenerPorCredencialesAsync(nombreUsuario, contrasena);
        }
    }
}
