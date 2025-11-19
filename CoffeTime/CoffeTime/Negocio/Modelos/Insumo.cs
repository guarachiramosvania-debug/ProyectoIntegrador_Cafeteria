using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace CoffeTime.Negocio.Modelos
{
    [Table("insumos")]
    public class Insumo : BaseModel
    {
        [PrimaryKey("id_insumo")]
        public long IdInsumo { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("unidad_medida")]
        public string UnidadMedida { get; set; }

        [Column("stock_actual")]
        public decimal StockActual { get; set; }

        [Column("stock_minimo")]
        public decimal StockMinimo { get; set; }

        [Column("costo_unitario")]
        public decimal CostoUnitario { get; set; }

        [Column("id_proveedor")]
        public long? IdProveedor { get; set; }
    }
}
