using Microsoft.AspNetCore.Mvc;
using Products.API.Services;
using Products.API_.Models;
using Products.API_.DTOs;

namespace Products.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService; // Declaración de la variable del servicio de productos

        public ProductsController(ProductService productService) // El constructor recibe el servicio desde el contenedor de .NET
        {
            _productService = productService; 
        }

        [HttpGet] // 1. GET api/products (nos trae todos los productos, con opción de filtrar por categoría o nombre)
        public async Task<IActionResult> GetAll([FromQuery] string? categoria = null, [FromQuery] string? nombre = null)
        {
            // Usamos await para esperar la respuesta de la base de datos, y pasamos los parámetros de categoría y nombre
            // para filtrar los productos si se especifican
            var products = await _productService.GetAllAsync(categoria, nombre); 
            return Ok(products);
        }

        [HttpGet("{id}")] // 2. GET api/products/{id} (obtiene un producto específico por su ID)
        public async Task <IActionResult> GetById(string id)
        {
            // El servicio se encarga de buscar el producto por su ID y lanza una excepción si no lo encuentra,
            // pero si lo encuentra devuelve el producto
            var product = await _productService.GetByIdAsync(id);

            return Ok(product);
        }

        [HttpPost] // 3. POST api/products (crea un nuevo producto, validando los datos de entrada)
        public async Task<IActionResult> Post([FromBody] ProductCreateDto newProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapeamos los datos que vienen del cliente (DTO) a nuestra entidad de producto que se guardará en la base de datos
            var product = new Product
            {
                Nombre = newProductDto.Nombre,
                Descripcion = newProductDto.Descripcion,
                Precio = newProductDto.Precio,
                Stock = newProductDto.Stock,
                Categoria = newProductDto.Categoria
            };

            // Guardamos el nuevo producto usando el servicio, que se encarga de asignar un ID y la fecha de creación,
            // y luego lo devolvemos en la respuesta con un código 201 Created
            var creado = await _productService.CreateAsync(product); 
            return StatusCode (201, creado);
        }


        [HttpPut("{id}")] // 4. PUT api/products/{id} (actualiza un producto existente, validando los datos de
                          // entrada y manejando el caso de producto no encontrado)
        public async Task <IActionResult> Put(string id, [FromBody] ProductUpdateDto updateProductDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Código 400: la solicitud es incorrecta
            }

            var product = new Product
            {
                Nombre = updateProductDto.Nombre,
                Descripcion = updateProductDto.Descripcion,
                Precio = updateProductDto.Precio,
                Stock = updateProductDto.Stock,
                Categoria = updateProductDto.Categoria
            };

            // El servicio se encarga de actualizar el producto y lanza una excepción si no lo encuentra
            await _productService.UpdateAsync(id, product); 

            return NoContent(); // Código 204: la actualización fue exitosa pero no se devuelve contenido en la respuesta
        }

        [HttpDelete("{id}")]// 5. DELETE api/products/{id} (elimina un producto por su ID, manejando el caso de
                            // producto no encontrado)
        public async Task<IActionResult> Delete(string id)
        {
            // El servicio se encarga de eliminar el producto y lanza una excepción si no lo encuentra
            await _productService.DeleteAsync(id);

            return NoContent(); // Devuelve 204 No Content indicando que la eliminación fue exitosa
        }


    }
}
