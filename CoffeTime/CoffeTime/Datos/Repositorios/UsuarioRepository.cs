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
        public async Task<bool> ActualizarUsuarioAsync(UsuarioModel usuario)
        {
            try
            {
                var response = await _client
                    .From<UsuarioModel>()
                    .Where(u => u.IdUsuario == usuario.IdUsuario)
                    .Update(usuario);   // ?? aquí va el modelo, no un Dictionary

                return response.Models.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR UPDATE: " + ex.Message);
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
