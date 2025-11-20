using CoffeTime.Datos.Repositorios;
using ProyectoIntegrador_Cafeteria.Negocio.Modelos;
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

        /// <summary>
        /// Obtiene todos los usuarios activos e inactivos desde la base de datos.
        /// </summary>
        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _usuarioRepository.ObtenerTodosAsync();
        }

        /// <summary>
        /// Crea un nuevo usuario con validaciones.
        /// </summary>
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

        /// <summary>
        /// Autentica un usuario por nombre de usuario y contraseña.
        /// </summary>
        public async Task<Usuario?> AutenticarAsync(string nombreUsuario, string contrasena)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(nombreUsuario);
            if (usuario != null && usuario.Contrasena == contrasena && usuario.Estado)
                return usuario;
            return null;
        }
    }
}