using Supabase;
using CoffeTime.Datos.Conexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        // ===================== LOGIN ==========================
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
                MessageBox.Show("ERROR LOGIN: " + ex.Message);
                return null;
            }
        }

        // ========== OBTENER POR NOMBRE_USUARIO (para validaciones / logout antiguo) ==========
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


        // ========== OBTENER POR ID (para dashboard, cierre por Id) ==========
        public async Task<UsuarioModel?> ObtenerPorIdAsync(long idUsuario)
        {
            try
            {
                var result = await _client
                    .From<UsuarioModel>()
                    .Filter("id_usuario", Supabase.Postgrest.Constants.Operator.Equals, idUsuario)
                    .Single();

                return result;
            }
            catch
            {
                return null;
            }
        }

        // ===================== CREAR USUARIO ====================
        public async Task<bool> CrearUsuarioAsync(UsuarioModel usuario)
        {
            try
            {
                usuario.Online = false; // por defecto
                await _client.From<UsuarioModel>().Insert(usuario);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR INSERT: " + ex.Message);
                return false;
            }
        }

        // ===================== ACTUALIZAR COMPLETO ====================
        // (se usa para editar, login, logout…)
        public async Task<bool> ActualizarOnlineAsync(long idUsuario, bool online)
        {
            try
            {
                // 1?? obtener el usuario completo
                var user = await ObtenerPorIdAsync(idUsuario);
                if (user == null)
                    return false;

                // 2?? modificar solo online
                user.Online = online;

                // 3?? actualizar enviando TODO el modelo
                var response = await _client
                    .From<UsuarioModel>()
                    .Where(u => u.IdUsuario == idUsuario)
                    .Update(user);

                return response.Models.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR UPDATE ONLINE: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> ActualizarOnlineSoloAsync(long idUsuario, bool online)
        {
            try
            {
                var response = await _client
                    .From<UsuarioModel>()
                    .Where(u => u.IdUsuario == idUsuario)
                    .Set(x => x.Online, online)   // ? ESTA ES LA CLAVE
                    .Update();

                return response.Models.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR UPDATE ONLINE: " + ex.Message);
                return false;
            }
        }




        public async Task<bool> ActualizarUsuarioAsync(UsuarioModel usuario)
        {
            try
            {
                var response = await _client
                    .From<UsuarioModel>()
                    .Where(u => u.IdUsuario == usuario.IdUsuario)
                    .Update(usuario);   // usa modelo, NO diccionario

                return response.Models.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR UPDATE PERFIL: " + ex.Message);
                return false;
            }
        }


        // ===================== OBTENER TODOS =====================
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

        // ===================== ELIMINAR ==========================
        public async Task<bool> EliminarUsuarioAsync(long id)
        {
            try
            {
                await _client
                    .From<UsuarioModel>()
                    .Where(u => u.IdUsuario == id)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR DELETE: " + ex.Message);
                return false;
            }
        }

    }
}
