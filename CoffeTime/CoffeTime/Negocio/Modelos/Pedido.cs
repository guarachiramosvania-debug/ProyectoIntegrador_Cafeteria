using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace CoffeTime.Negocio.Models
{
    [Table("pedidos")] // tabla público.pedidos
    public class Pedido : BaseModel
    {
        [PrimaryKey("id_pedido", false)]
        public long IdPedido { get; set; }

        [Column("numero_pedido")]
        public long NumeroPedido { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = string.Empty;

        [Column("metodo_pago")]
        public string MetodoPago { get; set; } = string.Empty;

        [Column("total")]
        public decimal Total { get; set; }

        [Column("id_usuario")]
        public long IdUsuario { get; set; }
    }
}
