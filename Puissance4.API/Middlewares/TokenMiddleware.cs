using Puissance4.Infrastructure.Security;
using System.Security.Claims;

namespace Puissance4.API.Middlewares
{
    public class TokenMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, TokenManager tokenManager)
        {
            string? accessToken = context.Request.Query["access_token"];
            if (accessToken != null)
            {
                ClaimsPrincipal? claims = tokenManager.Validate(accessToken);
                if (claims != null)
                {
                    context.User = claims; 
                }
            }
            await next(context);
        }
    }
}
