using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CoffeTime.Negocio.Modelos
{
    [Table("producto_insumo")]
    public class ProductoInsumo : BaseModel
    {
        [PrimaryKey("id_producto")]
        public long IdProducto { get; set; }

        [PrimaryKey("id_insumo")]
        public long IdInsumo { get; set; }

        [Column("cantidad")]
        public decimal Cantidad { get; set; }
    }
}
