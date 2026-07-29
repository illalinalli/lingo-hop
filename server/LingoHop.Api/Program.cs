using LingoHop.Api.Authentication;
using LingoHop.Api.ErrorHandling;
using LingoHop.Api.OpenApi;
using LingoHop.Application;
using LingoHop.Application.Abstractions.Security;
using LingoHop.Infrastructure;
using LingoHop.Infrastructure.Persistence;
using LingoHop.Infrastructure.Telegram;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

const string CorsPolicyName = "LingoHopMiniApp";

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Composition root. Inner layers first, then the adapters, then the web host.
// ---------------------------------------------------------------------------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTelegramUser, HttpCurrentTelegramUser>();

builder.Services.AddControllers();

builder.Services
    .AddAuthentication(TelegramAuthentication.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, TelegramAuthenticationHandler>(
        TelegramAuthentication.SchemeName,
        displayName: "Telegram Mini App",
        configureOptions: null);

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

builder.Services.AddOpenApi(options => options.AddDocumentTransformer<TelegramSecuritySchemeTransformer>());

// The mini app is served from a different origin than the API.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy(CorsPolicyName, policy =>
{
    if (allowedOrigins.Length == 0)
    {
        return;
    }

    policy.WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod();
}));

// Running behind nginx: trust the proxy headers so redirects and logs see the real scheme/host.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // Kestrel only ever listens on loopback here; nginx is the single trusted hop in front of it.
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

EnsureProductionIsLockedDown(app);

app.UseForwardedHeaders();
app.UseExceptionHandler();

if (IsApiDocumentationEnabled(app))
{
    app.MapOpenApi();
    app.UseSwaggerUI(swagger =>
    {
        swagger.SwaggerEndpoint("/openapi/v1.json", "LingoHop API v1");
        swagger.RoutePrefix = "swagger";
        swagger.DocumentTitle = "LingoHop API";
    });
}

app.UseCors(CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Liveness: the process is up.
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .AllowAnonymous()
    .ExcludeFromDescription();

// Readiness: the database is reachable.
app.MapGet("/health/ready", async (LingoHopDbContext database, CancellationToken cancellationToken) =>
        await database.Database.CanConnectAsync(cancellationToken)
            ? Results.Ok(new { status = "ready" })
            : Results.Json(new { status = "database-unreachable" }, statusCode: StatusCodes.Status503ServiceUnavailable))
    .AllowAnonymous()
    .ExcludeFromDescription();

await ApplyMigrationsIfRequestedAsync(app);

await app.RunAsync();

// Applies pending EF Core migrations when Database:ApplyMigrationsOnStartup is set.
// Convenient locally; in production prefer running the migration bundle as a separate
// deployment step so two app instances cannot migrate at the same time.
static async Task ApplyMigrationsIfRequestedAsync(WebApplication app)
{
    if (!app.Configuration.GetValue("Database:ApplyMigrationsOnStartup", false))
    {
        return;
    }

    await using var scope = app.Services.CreateAsyncScope();
    var database = scope.ServiceProvider.GetRequiredService<LingoHopDbContext>();
    await database.Database.MigrateAsync();

    app.Logger.LogInformation("Database migrations applied on startup.");
}

// Fails fast on a configuration that would leave production open: the unsigned development
// fallback must never be active there, and without a bot token nothing can authenticate.
static void EnsureProductionIsLockedDown(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        return;
    }

    var telegram = app.Services.GetRequiredService<IOptions<TelegramOptions>>().Value;

    if (telegram.AllowDevelopmentFallback)
    {
        throw new InvalidOperationException(
            "Telegram:AllowDevelopmentFallback must be false outside Development - it accepts unsigned requests.");
    }

    if (!telegram.HasBotToken)
    {
        throw new InvalidOperationException(
            "Telegram:BotToken is required outside Development. Set Telegram__BotToken in the environment.");
    }
}

// Swagger is always on in Development, and opt-in elsewhere via OpenApi:Enabled.
static bool IsApiDocumentationEnabled(WebApplication app) =>
    app.Environment.IsDevelopment() || app.Configuration.GetValue("OpenApi:Enabled", false);
