using CoffeTime.Datos.Repositorios;
using CoffeTime.Negocio.Modelos;

namespace CoffeTime.Negocio.Servicios
{
    public class ProductoService
    {
        private readonly ProductoRepository _repo;

        public ProductoService(ProductoRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Producto>> GetAll() => _repo.GetAll();

        public Task<Producto?> GetById(int id) => _repo.GetById(id);

        public Task<bool> Insert(Producto p) => _repo.Insert(p);

        public Task<bool> Update(Producto p) => _repo.Update(p);

        public Task<bool> Delete(int id) => _repo.Delete(id);
    }
}
