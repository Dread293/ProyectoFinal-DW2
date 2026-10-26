using System.Security.Claims;

namespace backend.Extensions;

public static class UserExtensions
{
    public static int? ClienteId(this ClaimsPrincipal user)
    {
        // JwtBearer puede mapear NameIdentifier a URI o dejarlo como nameid/sub,
        // según el emisor y la configuración del middleware. Revisamos las variantes.
        var valor = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user.FindFirst("nameid")?.Value
                    ?? user.FindFirst("sub")?.Value
                    ?? user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        return int.TryParse(valor, out var id) && id > 0 ? id : null;
    }
}
