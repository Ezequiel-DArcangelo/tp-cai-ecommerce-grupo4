using Orders.API.DTOs;

namespace Orders.API.Models
{
    
    public class Order
    {
        public Guid Id { get; set; } = Guid.Empty;              
        public Guid UsuarioId { get; set; } = Guid.Empty;      
        public List<OrderItemDTO> Items { get; set; } = new();  
        public decimal Total { get; set; }                   
        public string Estado { get; set; } = "Pendiente";       
        public string FechaCreacion { get; set; } = string.Empty; 
    }

    
    public class OrderEntity
    {
        public int Id { get; set; }                             
        public Guid UsuarioId { get; set; }                     
        public string Estado { get; set; } = "Pendiente";       
        public string FechaCreacion { get; set; } = string.Empty;
        public string? FechaActualizacion { get; set; }
    }
}