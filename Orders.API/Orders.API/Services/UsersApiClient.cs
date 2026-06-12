using Orders.API.Exceptions;

public class UsersApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UsersApiClient> _logger;

    public UsersApiClient(IHttpClientFactory httpClientFactory, ILogger<UsersApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<bool> ExisteUsuario(Guid usuarioId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("UsersApi");
            var respuesta = await client.GetAsync($"/api/users/{usuarioId}");
            return respuesta.StatusCode != System.Net.HttpStatusCode.NotFound;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar Users API para usuario {UsuarioId}", usuarioId);
            throw new BusinessRuleException("ORD-007", "Error al comunicarse con Users API.");
        }
    }
}
