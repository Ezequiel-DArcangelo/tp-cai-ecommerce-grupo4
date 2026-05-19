using Products.API_.Models;

namespace Products.API.Services

{
    public class ProductService

    {
        // Movimos la lista temporal de productos hacia acá 
        private static readonly List<Product> _products = new List<Product>();

        // Método para obtener todos los productos
        public IEnumerable<Product> GetAll()

        {
          
          return _products;
        
        }

        // Método para buscar un producto específico por su ID
        public Product GetById(Guid id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

    }

}
