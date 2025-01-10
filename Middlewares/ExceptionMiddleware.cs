using MySqlConnector;
using System.Net;
using System.Text.Json;

namespace InfoPoster_backend.Middlewares
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex).ConfigureAwait(false);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            string connectionString = "Server=localhost;Database=InfoPoster;User=root;Password=HvSY88Z9fajV;";

            try
            {
                using (var connectiondb = new MySqlConnection(connectionString))
                {
                    connectiondb.Open();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.InnerException == null ? exception.Message : exception.InnerException.Message,
            }.ToString()).ConfigureAwait(false);
        }
    }
}
