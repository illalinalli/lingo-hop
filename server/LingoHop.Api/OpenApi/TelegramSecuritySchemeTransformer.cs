using LingoHop.Api.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace LingoHop.Api.OpenApi;

/// <summary>
/// Declares the Telegram launch payload as the API's security scheme so Swagger UI offers an
/// Authorize box. Paste <c>tma &lt;initData&gt;</c> into it to call the API as a real user.
/// </summary>
internal sealed class TelegramSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description =
                "Telegram Mini App launch payload. Send the raw initData string prefixed with " +
                "\"tma \", for example: tma query_id=...&user=...&auth_date=...&hash=...",
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[TelegramAuthentication.SchemeName] = scheme;

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(TelegramAuthentication.SchemeName, document)] = [],
        });

        return Task.CompletedTask;
    }
}
