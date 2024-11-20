using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OidcProxy.Net.Jwt;
using OidcProxy.Net.OpenIdConnect;

namespace OidcProxy.Net.Endpoints;

internal static class MeEndpoint
{
    public static async Task<IResult> Get(HttpContext context,
        [FromServices] AuthSession authSession,
        [FromServices] ITokenParser tokenParser,
        [FromServices] IClaimsTransformation claimsTransformation)
    {
        if (!authSession.HasIdToken())
        {
            return Results.Unauthorized();
        }

        context.Response.Headers.CacheControl = $"no-cache, no-store, must-revalidate";

        var idToken = authSession.GetIdToken();
        var payload = tokenParser.ParseIdToken(idToken);
        if (payload == null)
        {
            return Results.Ok(new object());
        }
        
        var claims = await claimsTransformation.Transform(payload);
        return Results.Ok(claims);
    }
}
