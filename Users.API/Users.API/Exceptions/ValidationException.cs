namespace Users.API.Exceptions
{
    public class ValidationException : Exception
    {
        public string ErrorCode { get; set; }

        public ValidationException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}