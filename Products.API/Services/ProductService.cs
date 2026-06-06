using Products.API_.Exceptions;
using Products.API_.Models;
using Products.API_.DTOs;

namespace Products.API.Services

{
    public class ProductService

    {
        // Movimos la lista temporal de productos hacia acá 
        private static readonly List<Product> _products = new List<Product>();

        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Método para obtener todos los productos
        public IEnumerable<Product> GetAll(string? categoria, string? nombre)
        {
            var resultado = _products.AsEnumerable();

            if (!string.IsNullOrEmpty(categoria))
            {
                resultado = resultado.Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                resultado = resultado.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
            }

            return resultado.ToList();
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
        public void Add(ProductCreateDto newProductDto)
        {
            // Validamos si existe el nombre en la lista 
            bool yaExiste = _products.Any(p => p.Nombre.ToUpper() == newProductDto.Nombre.ToUpper() && p.Categoria.ToUpper() == newProductDto.Categoria.ToUpper());
            if (yaExiste)
            {
                throw new BusinessRuleException("PRD-003", $"Ya existe un producto registrado con el nombre '{newProductDto.Nombre}'.");
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Nombre = newProductDto.Nombre,
                Descripcion = newProductDto.Descripcion,
                Precio = newProductDto.Precio,
                Stock = newProductDto.Stock,
                Categoria = newProductDto.Categoria,
                FechaCreacion = DateTime.Now
            };

            _products.Add(product);
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
        public async Task DeleteAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                //Si no se encontró el producto a eliminar, detiene la ejecución con un 404
                throw new NotFoundException("PRD-001", $"Producto con ID {id} no se encontró.");
            }

            var client = _httpClientFactory.CreateClient();

            var respuesta = await client.GetFromJsonAsync<ResultadoOrden>($"https://localhost:7200/api/orders/check-product/{id}");

            if (respuesta != null && respuesta.TieneOrdenesActivas == true)
            {
                //Si el producto tiene órdenes asociadas, detiene la ejecución con un 400
                throw new BusinessRuleException("PRD-004", "El producto tiene órdenes activas y no puede eliminarse.");

                _products.Remove(product);
            }
        }
            public class ResultadoOrden
            {
                public bool TieneOrdenesActivas { get; set; }
            }
    }
}
