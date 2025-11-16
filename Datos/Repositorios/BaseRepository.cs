using ProyectoIntegrador_Cafeteria.Datos.Conexion;
using Postgrest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoIntegrador_Cafeteria.Datos.Repositorios
{
    public class BaseRepository<T> where T : BaseModel, new()
    {
        public async Task<List<T>> GetAll()
        {
            await SupabaseContext.InitializeAsync();
            var result = await SupabaseContext.Client.From<T>().Get();
            return result.Models;
        }

        public async Task<T> Insert(T entity)
        {
            await SupabaseContext.InitializeAsync();
            var response = await SupabaseContext.Client.From<T>().Insert(entity);
            return response.Models[0];
        }

        public async Task<T> Update(T entity)
        {
            await SupabaseContext.InitializeAsync();
            var response = await SupabaseContext.Client.From<T>().Update(entity);
            return response.Models[0];
        }

        public async Task Delete(T entity)
        {
            await SupabaseContext.InitializeAsync();
            await SupabaseContext.Client.From<T>().Delete(entity);
        }
    }
}
