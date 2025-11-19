using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CoffeTime.Negocio.Modelos
{
    [Table("productos")]
    public class Producto : BaseModel
    {
        [PrimaryKey("id_producto")]
        public long IdProducto { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("categoria")]
        public string Categoria { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }
    }
}
