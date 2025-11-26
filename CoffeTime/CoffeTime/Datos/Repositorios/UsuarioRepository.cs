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

        // ========== ACTUALIZAR PERFIL (NO TOCAR ONLINE) =========
        public async Task<bool> ActualizarPerfilAsync(UsuarioModel usuario)
        {
            try
            {
                var dic = new Dictionary<string, object>
                {
                    ["nombre"] = usuario.Nombre,
                    ["apellido"] = usuario.Apellido,
                    ["usuario"] = usuario.NombreUsuario,
                    ["rol"] = usuario.Rol.ToLower(),
                    ["estado"] = usuario.Estado,
                    ["ultimo_login"] = usuario.UltimoLogin
                };

                var response = await _client
                    .From<UsuarioModel>()
                    .Where(u => u.IdUsuario == usuario.IdUsuario)
                    .Update(dic);  // ?? ESTE ES EL MÉTODO CORRECTO

                return response.Models.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR UPDATE PERFIL: " + ex.Message);
                return false;
            }
        }



        // ========= ACTUALIZAR SOLO EL CAMPO ONLINE ==========
        public async Task<bool> ActualizarOnlineAsync(long idUsuario, bool online)
        {
            try
            {
                var dic = new Dictionary<string, object>
                {
                    ["online"] = online
                };

                var response = await _client
                    .From<UsuarioModel>()
                    .Where(u => u.IdUsuario == idUsuario)
                    .Update(dic);   // ?? MISMO MÉTODO

                return response.Models.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR UPDATE ONLINE: " + ex.Message);
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
