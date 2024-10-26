namespace Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                var response = new { message = ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (InvalidModelStateException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                var errorsList = ex.AggregatedExceptions.Select(ss =>
                new ErrorInfo () {
                    Field = ss.Key,
                    Message = ss.Value,
                }).ToList();
                var response = new { message = ex.Message, errors = errorsList };
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new { message = ex.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}