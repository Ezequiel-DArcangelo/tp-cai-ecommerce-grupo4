namespace Orders.API.DTOs
{
    public class OrderItemDTO
    {
        public int ProductoId { get; set; }   // ID del producto
        public int Cantidad { get; set; }     // Cantidad solicitada
    }
}