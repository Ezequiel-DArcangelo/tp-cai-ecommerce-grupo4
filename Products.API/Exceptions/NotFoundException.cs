namespace Products.API_.Exceptions
{
    // PRD-001: Excepción para indicar que un recurso no fue encontrado
    public class NotFoundException(string errorCode, string message) : Exception(message)
    {
      public string ErrorCode { get; } = errorCode;
    }
}
