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
                return BadRequest(ModelState);
            }

            var success = _productService.Update(id, updatedProduct);
            if (!success)
            {
                return NotFound();
            }

            return NoContent(); // Código 204: la actualización fue exitosa pero no devuelve contenido
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
