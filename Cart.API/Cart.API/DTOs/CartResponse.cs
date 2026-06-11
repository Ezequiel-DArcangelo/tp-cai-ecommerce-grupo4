namespace Cart.API.DTOs
{
    public class CartResponse
    {
        public Guid UsuarioId { get; set; }
        public List<CartItemDTO> Items { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}