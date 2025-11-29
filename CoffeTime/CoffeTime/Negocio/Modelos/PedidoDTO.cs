using System;
using System.Collections.Generic;

namespace CoffeTime.Negocio.Modelos.DTO
{
    public class PedidoDTO
    {
        public long IdPedido { get; set; }
        public string NombrePedido { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string FechaHora => Fecha.ToString("dd/MM/yyyy HH:mm");
        public string Estado { get; set; } = "";
        public string MetodoPago { get; set; } = "";
        public decimal Total { get; set; }
        public List<string> Productos { get; set; } = new();
    }
}
