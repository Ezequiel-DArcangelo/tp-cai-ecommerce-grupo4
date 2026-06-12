using Cart.API.Exceptions;

namespace Cart.API.Services
{
    public class ProductsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductsApiClient> _logger;

        public ProductsApiClient(HttpClient httpClient, ILogger<ProductsApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ProductDto?> GetProductAsync(string productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/products/{productId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("CRT-002", $"El producto con ID {productId} no existe.");

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar Products API para producto {ProductoId}", productId);
                throw new Exceptions.BusinessRuleException("CRT-005", "Error al comunicarse con Products API.");
            }
        }
    }

    public class ProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Stock { get; set; }
    }
}