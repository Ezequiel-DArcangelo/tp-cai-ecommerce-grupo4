using Orders.API.DTOs;

namespace Orders.API.Models
{
    public class Order
    {
        public string Id { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string FechaCreacion { get; set; } = string.Empty;
    }
}