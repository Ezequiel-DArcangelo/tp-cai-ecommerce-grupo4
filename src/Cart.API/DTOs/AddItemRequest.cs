namespace Cart.API.DTOs
{
    public class AddItemRequest
    {
        public string ProductoId { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}