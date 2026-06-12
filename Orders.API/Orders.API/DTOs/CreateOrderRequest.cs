namespace Orders.API.DTOs
{
    public class CreateOrderRequest
    {
        public string UsuarioId { get; set; } = string.Empty;
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }

}
