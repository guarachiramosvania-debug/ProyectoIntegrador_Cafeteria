using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace CoffeTime.Negocio.Modelos
{
    [Table("movimiento_inventario")]
    public class MovimientoInventario : BaseModel
    {
        [PrimaryKey("id_movimiento")]
        public long IdMovimiento { get; set; }

        [Column("id_insumo")]
        public long IdInsumo { get; set; }

        [Column("tipo_movimiento")]
        public string TipoMovimiento { get; set; }

        [Column("cantidad")]
        public decimal Cantidad { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("usuario_responsable")]
        public long UsuarioResponsable { get; set; }

        [Column("costo_total")]
        public decimal? CostoTotal { get; set; }
    }
}
