using Orders.API.Exceptions;

namespace Orders.API.Services
{
    public class ProductsApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductsApiClient> _logger;

        public ProductsApiClient(IHttpClientFactory httpClientFactory, ILogger<ProductsApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ProductoDto?> GetProducto(string productoId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ProductsApi"); 
                var respuesta = await client.GetAsync($"/api/products/{productoId}");

                if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                respuesta.EnsureSuccessStatusCode();

                return await respuesta.Content.ReadFromJsonAsync<ProductoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar Products API para producto {ProductoId}", productoId);
                throw new BusinessRuleException("ORD-007", "Error al comunicarse con Products API.");
            }
        }
    }

    public class ProductoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Precio { get; set; }
    }
}
