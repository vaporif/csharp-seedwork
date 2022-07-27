namespace ConferencePlanner.Api
{
    public class GraphErrorFilter : IErrorFilter
    {
        private readonly ILogger _logger;

        public GraphErrorFilter(ILogger<IErrorFilter> logger) => _logger = logger;

        public IError OnError(IError error)
        {
            _logger.LogError(error.Exception, error.Message);
            return error;
        }
    }
}
