using NoteAPI.Exceptions;

namespace NoteAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (NotFoundException notFoundException)
            {
                context.Response.StatusCode = 404; // NOT FOUND
                await context.Response.WriteAsync(notFoundException.Message);
            }
            catch (ForbidException forbidException)
            {
                context.Response.StatusCode = 403; // FORBIDDEN
                await context.Response.WriteAsync(forbidException.Message);
            }
            catch (Exception exc)
            {
                _logger.LogError($"[Error] {exc.Message}");

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}