namespace Orders.API.DTOs
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public List<OrderItemDTO> Items { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } // Pendiente, Confirmada, Enviada, Entregada
        public DateTime FechaCreacion { get; set; }
    }
}
