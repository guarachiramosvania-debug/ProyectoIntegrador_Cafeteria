using CoffeTime.Datos.Conexion;
using CoffeTime.Negocio.Models;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeTime.Datos.Repositorios
{
    public class PedidoRepository
    {
        private readonly Client _db;

        public PedidoRepository()
        {
            _db = SupabaseContext.Client;
        }

        // ================================
        // 1) Obtener lista de pedidos
        // ================================
        public async Task<List<PedidoVistaDto>> ObtenerPedidosAsync()
        {
            try
            {
                var resp = await _db
                    .From<Pedido>()
                    .Get();

                var pedidos = resp.Models.ToList();

                // DEBUG: para ver si realmente viene algo
                System.Diagnostics.Debug.WriteLine($"[PedidoRepository] pedidos en BD: {pedidos.Count}");

                var lista = pedidos
                    .OrderByDescending(p => p.IdPedido)
                    .Select(p => new PedidoVistaDto
                    {
                        IdPedido = p.IdPedido,
                        NumeroPedido = p.NumeroPedido,
                        NombrePedido = $"Pedido #{p.NumeroPedido}",
                        Estado = p.Estado,
                        FechaHora = p.Fecha.ToString("dd/MM/yyyy HH:mm"),
                        MetodoPago = p.MetodoPago,
                        Total = p.Total.ToString("0.00"),
                        Productos = new List<string>() // de momento vacío
                    })
                    .ToList();

                return lista;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR obteniendo pedidos: " + ex);
                return new List<PedidoVistaDto>();
            }
        }

        // ================================
        // 2) Cancelar pedido
        // ================================
        public async Task<bool> CancelarPedidoAsync(long idPedido)
        {
            try
            {
                var resp = await _db
                    .From<Pedido>()
                    .Where(p => p.IdPedido == idPedido)
                    .Get();

                var pedido = resp.Models.FirstOrDefault();
                if (pedido == null)
                    return false;

                pedido.Estado = "Cancelado";

                await _db.From<Pedido>().Update(pedido);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR cancelando pedido: " + ex);
                return false;
            }
        }

        internal async Task<bool> CrearPedidoAsync(int numeroPedido, string metodoPago, long idUsuario, List<(int idProducto, int cantidad)> items)
        {
            throw new NotImplementedException();
        }

        // De momento NO creamos pedidos desde aquí
    }
}
