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
        public IActionResult Get()
        {
            return Ok(_productService.GetAll());
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
            var product = _productService.GetById(id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] Product updatedProduct) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Código 400: la solicitud es incorrecta
            }

            var success = _productService.Update(id, updatedProduct);
            if (!success)
            {
                return NotFound(); // Código 404: el producto con el ID especificado no existe
            }

            var product = _productService.GetById(id); // Obtiene el producto actualizado para devolverlo en la respuesta

            return Ok(product); // Devuelve 200 OK con el producto actualizado en el cuerpo de la respuesta
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var success = _productService.Delete(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }


    }
}
