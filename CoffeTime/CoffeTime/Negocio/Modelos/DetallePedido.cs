using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CoffeTime.Negocio.Modelos
{
    [Table("detalle_pedido")]
    public class DetallePedido : BaseModel
    {
        [PrimaryKey("id_detalle")]
        public long IdDetalle { get; set; }

        [Column("id_pedido")]
        public long IdPedido { get; set; }

        [Column("id_producto")]
        public long IdProducto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }
    }
}
