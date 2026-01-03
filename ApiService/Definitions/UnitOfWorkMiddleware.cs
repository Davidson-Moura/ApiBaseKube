using ApiService.Domain.Databases;

namespace ApiService.Definitions
{
    public class UnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;
        public UnitOfWorkMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(
            HttpContext context,
            IConnectionContext connectionContext)
        {
            await _next.Invoke(context);

            if (connectionContext.PostgreConnection.ThereAreChanges)
                await connectionContext.PostgreConnection.SaveChangesAsync();
        }
    }
}
