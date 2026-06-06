using Microsoft.AspNetCore.Mvc;
using Products.API.Services;
using Products.API_.Models;

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

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? categoria, [FromQuery] string? nombre)
        {
            var products = _productService.GetAll(categoria, nombre); // El servicio se encarga de filtrar los productos según los parámetros de consulta
            return Ok(products);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Product newProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _productService.Add(newProduct); // El servicio se encarga de asignarle ID, fecha y de guardarlo
            return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var product = _productService.GetById(id);// El servicio se encarga de buscar el producto por ID y lanzar una excepción si no lo encuentra

            return Ok(product);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] Product updatedProduct) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Código 400: la solicitud es incorrecta
            }

            _productService.Update(id, updatedProduct);// El servicio se encarga de actualizar el producto y lanza una excepción si no lo encuentra

            var product = _productService.GetById(id); // Obtiene el producto actualizado para devolverlo en la respuesta

            return Ok(product); // Devuelve 200 OK con el producto actualizado en el cuerpo de la respuesta
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);// El servicio se encarga de eliminar el producto y lanza una excepción si no lo encuentra

            return NoContent(); // Devuelve 204 No Content indicando que la eliminación fue exitosa
        }


    }
}
