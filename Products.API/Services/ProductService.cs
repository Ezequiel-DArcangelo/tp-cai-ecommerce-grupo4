using Products.API_.Exceptions;
using Products.API_.Models;
using Products.API_.DTOs;

namespace Products.API.Services

{
    public class ProductService

    { 
        private readonly ProductRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor que recibe el repositorio y el HttpClientFactory a través de inyección de dependencias
        public ProductService(ProductRepository repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
        }

        // Método para obtener todos los productos de la base de datos
        public async Task<IEnumerable<Product>> GetAllAsync(string? categoria = null, string? nombre = null) 
        {
            var productos = await _repository.GetAllAsync(); // Obtengo todos los productos del repositorio
            if (!string.IsNullOrEmpty(categoria))
            {
                // Si se especifica una categoría, filtro los productos por esa categoría
                productos = productos.Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase)); 
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                // Si se especifica un nombre, filtro los productos por ese nombre (ignorando mayúsculas/minúsculas)
                productos = productos.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
            }

            return productos; // Devuelvo la lista de productos filtrada (o sin filtrar si no se especificaron criterios)
        }

        // Método para buscar un producto específico por su ID
        public async Task<Product> GetByIdAsync(string id)
        {
            var productos = await _repository.GetAllAsync();
            var producto = productos.FirstOrDefault(p => p.Id == id);

            if (producto == null)
            {
                throw new NotFoundException("PRD-001", $"Producto con ID {id} no existe.");
            }

            return producto;
        }

        public async Task<Product> CreateAsync(Product product) // Método para crear un nuevo producto en la base de datos
        {
            var existingProducts = await _repository.GetAllAsync(); // Obtengo todos los productos para validar que no este repetido
          
            if (existingProducts.Any(p => p.Nombre.Trim().ToLower() == product.Nombre.Trim().ToLower()))
            {
                // Si ya existe un producto con el mismo nombre y categoría, lanzo una excepción de regla de negocio
                throw new BusinessRuleException("PRD-003",
                $"Ya existe un producto registrado con el nombre '{product.Nombre}' en la categoría '{product.Categoria}'.");
            }

            // Se generan los datos automáticos antes de guardar el producto en la base de datos
            product.Id = Guid.NewGuid().ToString(); 
            product.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"); 

            await _repository.CreateAsync(product); // Le pido al repositorio que guarde el nuevo producto en la base de datos
            return product; 
        }


        //Método para actualizar un producto existente 
        public async Task UpdateAsync(string id, Product product)
        {
            var existingProduct = await GetByIdAsync(id); // Primero busco el producto por su ID para validar que exista
           
            product.Id = id; // Aseguro que el ID del producto a actualizar sea el mismo que el ID del producto existente
            product.CreatedAt = existingProduct.CreatedAt; // Mantengo la fecha de creación original del producto

            await _repository.UpdateAsync(product); // Le pido al repositorio que actualice el producto en la base de datos
        }

        //Método para eliminar un producto
        public async Task DeleteAsync(string id)
        {
            var product = await GetByIdAsync(id);

            var client = _httpClientFactory.CreateClient();

            var respuesta = await client.GetAsync($"https://localhost:56970/api/orders/check-product/{id}");

            // Validamos que el microservicio haya respondido un 200 OK antes de intentar leer
            if (respuesta.IsSuccessStatusCode)
            {
                // 2. Leemos la respuesta usando el DTO correcto
                var body = await respuesta.Content.ReadFromJsonAsync<TieneOrdenesDto>();

                // 3. Chequeamos el booleano
                if (body != null && body.TieneOrdenesActivas)
                {
                    throw new BusinessRuleException("PRD-004", "El producto tiene órdenes activas y no puede eliminarse.");
                }
            }
            else
            {
                throw new Exception("Error al intentar comunicarse con el servicio de órdenes para validar el borrado.");
            }

            await _repository.DeleteAsync(id);
        }
        public class TieneOrdenesDto
        {
          public bool TieneOrdenesActivas { get; set; }
        }
    }
}
