using Supabase;
using CoffeTime.Datos.Conexion;
using CoffeTime.Negocio.Modelos;

namespace CoffeTime.Datos.Repositorios
{
    public class ProveedorRepository
    {
        private readonly Supabase.Client _client;

        public ProveedorRepository()
        {
            _client = SupabaseContext.Client;
        }

        public async Task<List<Proveedor>> GetAll()
        {
            var result = await _client.From<Proveedor>().Get();
            return result.Models;
        }

        public async Task<bool> Insert(Proveedor proveedor)
        {
            var result = await _client
                .From<Proveedor>()
                .Insert(proveedor);

            return result.Models.Count > 0;
        }

        public async Task<bool> Update(Proveedor proveedor)
        {
            var result = await _client
                .From<Proveedor>()
                .Where(p => p.Id == proveedor.Id)
                .Update(proveedor);

            return result.Models.Count > 0;
        }

        public async Task<bool> Delete(long id)
        {
            await _client
                .From<Proveedor>()
                .Where(p => p.Id == id)
                .Delete();

            return true;
        }

        public async Task<Proveedor?> GetById(long id)
        {
            var result = await _client
                .From<Proveedor>()
                .Where(p => p.Id == id)
                .Single();

            return result;
        }
    }
}
