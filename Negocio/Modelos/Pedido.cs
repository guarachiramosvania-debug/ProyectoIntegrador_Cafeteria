using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace ProyectoIntegrador_Cafeteria.Negocio.Modelos
{
    [Table("pedidos")]
    public class Pedido : BaseModel
    {
        [PrimaryKey("id_pedido")]
        public long IdPedido { get; set; }

        [Column("numero_pedido")]
        public long NumeroPedido { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("metodo_pago")]
        public string MetodoPago { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("id_usuario")]
        public long IdUsuario { get; set; }
    }
}
