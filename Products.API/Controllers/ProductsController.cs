using Microsoft.AspNetCore.Mvc;
using Products.API_.Models;

namespace Products.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Es una lista temporal
        private static List<Product> _products = new List<Product>();

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _products;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Product newProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            newProduct.Id = Guid.NewGuid();
            newProduct.FechaCreacion = DateTime.Now;
            _products.Add(newProduct);
            return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] Product productUpdate) 
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null) return NotFound();

            // Se actualizan los campos del modelo que tenemos en Product.cs
            existingProduct.Nombre = productUpdate.Nombre;
            existingProduct.Descripcion = productUpdate.Descripcion; 
            existingProduct.Precio = productUpdate.Precio;
            existingProduct.Stock = productUpdate.Stock;
            existingProduct.Categoria = productUpdate.Categoria; 

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            _products.Remove(product);
            return NoContent();
        }


    }
}
