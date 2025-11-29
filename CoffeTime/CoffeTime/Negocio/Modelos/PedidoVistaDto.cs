using System.Collections.Generic;

namespace CoffeTime.Negocio.Models
{
    public class PedidoVistaDto
    {
        public long IdPedido { get; set; }
        public long NumeroPedido { get; set; }

        public string NombrePedido { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string FechaHora { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;

        public List<string> Productos { get; set; } = new List<string>();
    }
}
