using CoffeTime.Datos.Conexion;
using Supabase.Postgrest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeTime.Datos.Repositorios
{
    public class BaseRepository<T> where T : BaseModel, new()
    {
        public async Task<List<T>> GetAll()
        {
            var result = await SupabaseContext.Client.From<T>().Get();
            return result.Models;
        }

        public async Task<T> Insert(T entity)
        {
            var response = await SupabaseContext.Client.From<T>().Insert(entity);
            return response.Models[0];
        }

        public async Task<T> Update(T entity)
        {
            var response = await SupabaseContext.Client.From<T>().Update(entity);
            return response.Models[0];
        }

        public async Task Delete(T entity)
        {
            await SupabaseContext.Client.From<T>().Delete(entity);
        }
    }
}
