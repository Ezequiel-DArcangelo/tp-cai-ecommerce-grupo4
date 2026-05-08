using Microsoft.AspNetCore.Mvc;
using Products.API_.Models;

namespace Products.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Esto es una lista temporal para probar que la API funciona
        private static List<Product> _products = new List<Product>();

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _products;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Product newProduct)
        {
            newProduct.Id = Guid.NewGuid();
            newProduct.FechaCreacion = DateTime.Now;
            _products.Add(newProduct);
            return Ok(newProduct);
        }
    }
}
