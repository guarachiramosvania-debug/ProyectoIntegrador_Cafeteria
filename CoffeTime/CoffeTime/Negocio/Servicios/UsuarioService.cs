using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;
using ProyectoIntegrador_Cafeteria.Negocio.Modelos;
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

        public async Task<(bool exito, string mensaje)> CrearUsuarioAsync(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario) ||
                string.IsNullOrWhiteSpace(usuario.Contrasena))
                return (false, "Usuario o contraseña no válidos");

            var existe = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(usuario.NombreUsuario);
            if (existe != null)
                return (false, "Nombre de usuario ya existe");

            var creado = await _usuarioRepository.CrearUsuarioAsync(usuario);
            return (creado, creado ? "Usuario creado" : "Error al crear usuario");
        }

        public async Task<Usuario?> AutenticarAsync(string nombreUsuario, string contrasena)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(nombreUsuario);
            if (usuario != null && usuario.Contrasena == contrasena && usuario.Estado)
                return usuario;
            return null;
        }
    }
}