namespace Products.API_.Exceptions
{
    // PRD-003 y PRD-004: Excepciones para las violaciones de reglas de negocio
    public class BusinessRuleException(string errorCode, string message) : Exception(message)
    {
        public string ErrorCode { get; } = errorCode;
    }
}
