using System.ComponentModel.DataAnnotations;
namespace Products.API_.Models
{
    public class Product
    {
        public string Id { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public string Categoria { get; set; } = string.Empty;

        public string CreatedAt { get; set; } = string.Empty;
    }
}
