namespace Users.API.Exceptions
{
    public class NotFoundException : Exception
    {
        public string ErrorCode { get; set; }

        public NotFoundException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}