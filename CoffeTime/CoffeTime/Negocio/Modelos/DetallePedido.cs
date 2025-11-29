using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Newtonsoft.Json;

namespace CoffeTime.Negocio.Modelos
{
    [Table("detalle_pedido")]
    public class DetallePedido : BaseModel
    {
        [PrimaryKey("id_detalle", false)]
        public long IdDetalle { get; set; }

        [Column("id_pedido")]
        public long IdPedido { get; set; }

        [Column("id_producto")]
        public long IdProducto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        // 👇 Campo solo para mostrar en UI, Supabase lo ignorará
        [JsonIgnore]
        public string NombreProducto { get; set; }
    }
}
