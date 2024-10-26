namespace Middlewares
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }

        protected BadRequestException(string message, Dictionary<string, string> aggregatedExceptions)
        : base(message)
        {
            AggregatedExceptions = aggregatedExceptions;
        }
        public Dictionary<string, string> AggregatedExceptions { get; set; }
    }
}
