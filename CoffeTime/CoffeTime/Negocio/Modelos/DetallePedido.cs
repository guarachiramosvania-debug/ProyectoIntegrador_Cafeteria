using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CoffeTime.Negocio.Modelos
{
    [Table("detalle_pedido")]
    public class DetallePedido : BaseModel
    {
        [PrimaryKey("id_detalle")]
        public int IdDetalle { get; set; }

        [Column("id_pedido")]
        public int IdPedido { get; set; }

        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }
    }
}
