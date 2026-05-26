namespace Orders.API.DTOs
{
    public class CreateOrderRequest
    {
        public int UsuarioId { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }

}
