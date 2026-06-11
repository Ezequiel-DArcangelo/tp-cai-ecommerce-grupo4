using Cart.API.DTOs;

namespace Cart.API.Models
{
    public class Cart
    {
        public Guid UsuarioId { get; set; }
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
        public DateTime FechaActualizacion { get; set; }
    }
}