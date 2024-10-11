using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Runtime.Versioning;

namespace HealthChecks;

[SupportedOSPlatform("windows")]
public static class HealtChecksRegistrations
{
    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddSqlServer(
                    configuration["ConnectionStrings:HealthChecksConnectionString"]!,
                    name: "Database Health Check",
                    tags: ["SQL Server", "Database"]);

        hcBuilder.AddCheck<DirectoryHealthCheck>(
                    name: "Directory Health Check",
                    tags: ["Directory"]);

        return services;
    }

    public static IApplicationBuilder UseHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        }).AllowAnonymous();

        return app;
    }
}
