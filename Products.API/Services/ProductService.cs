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

        //Método para agregar un nuevo producto a la lista de productos
        public void Add(Product newProduct)
        {
            newProduct.Id = Guid.NewGuid();
            newProduct.FechaCreacion = DateTime.Now;
            _products.Add(newProduct);
        }

        //Método para actualizar un producto existente 
        public bool Update(Guid id, Product updatedProduct)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return false; //No se encontró el producto a actualizar
            }

            //Actualizamos los campos correspondientes 
            existingProduct.Nombre = updatedProduct.Nombre;
            existingProduct.Descripcion = updatedProduct.Descripcion;
            existingProduct.Precio = updatedProduct.Precio;
            existingProduct.Stock = updatedProduct.Stock;
            existingProduct.Categoria = updatedProduct.Categoria;

            return true; //Producto actualizado exitosamente
        }

        //Método para eliminar un producto
        public bool Delete(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return false; //Si no se encontró el producto a eliminar
            }
            _products.Remove(product);
            return true; //Producto eliminado exitosamente
        }
    }
}
