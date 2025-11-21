using CoffeTime.Datos.Conexion;
using CoffeTime.Negocio.Modelos;
using Supabase;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;


// ?? USAMOS SOLO ESTE MODELO PARA EVITAR DUPLICADOS
using UsuarioModel = CoffeTime.Negocio.Modelos.Usuario;

namespace CoffeTime.Datos.Repositorios
{
    public class UsuarioRepository
    {
        private readonly Client _client;

        public UsuarioRepository()
        {
            _client = SupabaseContext.Client;
        }

        public UsuarioRepository(Client client)
        {
            _client = client;
        }

        // LOGIN REAL
        public async Task<UsuarioModel?> ObtenerPorCredencialesAsync(string usuario, string contrasena)
        {
            try
            {
                var result = await _client
                    .From<UsuarioModel>()
                    .Filter("usuario", Supabase.Postgrest.Constants.Operator.Equals, usuario)
                    .Filter("contrasena", Supabase.Postgrest.Constants.Operator.Equals, contrasena)
.Filter("estado", Supabase.Postgrest.Constants.Operator.Equals, "true")
                    .Get();

                return result.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR LOGIN: {ex.Message}");
                return null;
            }
        }



        // Obtener usuario por nombre
        public async Task<UsuarioModel?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            try
            {
                var response = await _client
                    .From<UsuarioModel>()
                    .Filter("usuario", Supabase.Postgrest.Constants.Operator.Equals, nombreUsuario)
                    .Single();

                return response;
            }
            catch
            {
                return null;
            }
        }

        // Crear usuario
        public async Task<bool> CrearUsuarioAsync(UsuarioModel usuario)
        {
            try
            {
                await _client.From<UsuarioModel>().Insert(usuario);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Obtener todos
        public async Task<List<UsuarioModel>> ObtenerTodosAsync()
        {
            try
            {
                var response = await _client.From<UsuarioModel>().Get();
                return response.Models;
            }
            catch
            {
                return new List<UsuarioModel>();
            }
        }
    }
}
