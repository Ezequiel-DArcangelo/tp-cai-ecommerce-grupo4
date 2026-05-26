using Orders.API.DTOs;

namespace Orders.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public List<OrderItemDTO> Items { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}