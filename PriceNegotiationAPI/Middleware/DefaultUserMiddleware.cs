using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Text.Json;

namespace PriceNegotiationAPI.Middleware
{
    public class DefaultUserMiddleware
    {
        private readonly RequestDelegate _next;

        public DefaultUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<IdentityUser> userManager)
        {
            if (context.Request.Path.Equals("/User/login", StringComparison.OrdinalIgnoreCase) && context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
                {
                    var loginRequest = await DeserializeJsonAsync(context.Request);

                    var requestedUser = loginRequest != null ? await userManager.FindByEmailAsync(loginRequest.Email) : null;

                    if (requestedUser != null && await userManager.IsInRoleAsync(requestedUser, "User"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Access forbidden.");
                        return;
                    }
                }
            }

            await _next(context);
        }

        private async Task<LoginRequest?> DeserializeJsonAsync(HttpRequest request)
        {
            request.EnableBuffering();

            try
            {
                var loginRequest = await request.ReadFromJsonAsync<LoginRequest>();
                request.Body.Position = 0;
                return loginRequest;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                request.Body.Position = 0;
                return null;
            }
        }
    }
}
