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

namespace Cart.API.Models
{
    public class CartEntity
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string? UpdatedAt { get; set; }
    }

    public class CartItemEntity
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string ProductoId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string? UpdatedAt { get; set; }
    }
}