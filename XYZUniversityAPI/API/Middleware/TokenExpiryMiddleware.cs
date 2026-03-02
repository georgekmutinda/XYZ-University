using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
namespace XYZUniversityAPI.Middleware
{
    public class TokenExpiryMiddleware
{
    private readonly RequestDelegate _next;

    public TokenExpiryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip login or public endpoints
        var path = context.Request.Path.Value?.ToLower();
        if (path != null && path.Contains("/auth/login") )
        {
            await _next(context);
            return;
        }

        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                

                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        statusCode = 401,
                        message = "Token expired. Please login again."
                    });
                    return;
                }
            }
        }

        await _next(context);
    }
}


}

