using Products.API_.Exceptions;
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
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                throw new NotFoundException("PRD-001", $"Producto con ID {id} no existe.");
            }

            return product;
        }

        //Método para agregar un nuevo producto a la lista de productos
        public void Add(Product newProduct)
        {
            // Validamos si existe el nombre en la lista 
            var yaExiste = _products.Any(p => p.Nombre.ToUpper() == newProduct.Nombre.ToUpper());
            if (yaExiste)
            {
                throw new BusinessRuleException("PRD-003", $"Ya existe un producto registrado con el nombre '{newProduct.Nombre}'.");
            }

            newProduct.Id = Guid.NewGuid();
            newProduct.FechaCreacion = DateTime.Now;
            _products.Add(newProduct);
        }

        //Método para actualizar un producto existente 
        public void Update(Guid id, Product updatedProduct)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                throw new NotFoundException("PRD-001", $"Producto con ID {id} no se encontró."); //Si no se encontró el producto a actualizar
            }

            //Actualizamos los campos correspondientes 
            existingProduct.Nombre = updatedProduct.Nombre;
            existingProduct.Descripcion = updatedProduct.Descripcion;
            existingProduct.Precio = updatedProduct.Precio;
            existingProduct.Stock = updatedProduct.Stock;
            existingProduct.Categoria = updatedProduct.Categoria;

        }

        //Método para eliminar un producto
        public void Delete(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                //Si no se encontró el producto a eliminar, detiene la ejecución con un 404
                throw new NotFoundException("PRD-001", $"Producto con ID {id} no se encontró."); 
            }
            _products.Remove(product);
        }
    }
}
