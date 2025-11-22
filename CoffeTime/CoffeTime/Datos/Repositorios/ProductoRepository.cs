using Supabase;
using CoffeTime.Datos.Conexion;
using CoffeTime.Negocio.Modelos;

namespace CoffeTime.Datos.Repositorios
{
    public class ProductoRepository
    {
        private readonly Supabase.Client _client;

        // ?? Constructor correcto, SIN parámetros
        public ProductoRepository()
        {
            _client = SupabaseContext.Client;
        }

        // GET ALL
        public async Task<List<Producto>> GetAll()
        {
            var result = await _client
                .From<Producto>()
                .Get();

            return result.Models;
        }

        // GET BY ID
        public async Task<Producto?> GetById(int id)
        {
            var result = await _client
                .From<Producto>()
                .Where(x => x.Id == id)
                .Single();

            return result;
        }

        // INSERT
        public async Task<bool> Insert(Producto p)
        {
            var result = await _client
                .From<Producto>()
                .Insert(p);

            return result.Models.Count > 0;
        }

        // UPDATE
        public async Task<bool> Update(Producto p)
        {
            var result = await _client
                .From<Producto>()
                .Where(x => x.Id == p.Id)
                .Update(p);

            return result.Models.Count > 0;
        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            await _client
                .From<Producto>()
                .Where(x => x.Id == id)
                .Delete();

            return true;
        }
    }
}
