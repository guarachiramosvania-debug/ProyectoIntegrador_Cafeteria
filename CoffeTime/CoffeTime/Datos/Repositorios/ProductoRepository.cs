using CoffeTime.Negocio.Modelos;
using Supabase;

namespace CoffeTime.Datos.Repositorios
{
    public class ProductoRepository : BaseRepository<Producto>
    {
        private Client client;

        public ProductoRepository(Client client)
        {
            this.client = client;
        }
    }
}
