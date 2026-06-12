namespace Users.API.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public string ErrorCode { get; set; }

        public BusinessRuleException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}