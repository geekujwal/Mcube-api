namespace Middlewares
{
    public class InvalidModelStateException : BadRequestException
    {
        public InvalidModelStateException(string message, Dictionary<string, string> innerException) : base(message, innerException)
        {
            
        }

    }
}
