using System.IdentityModel.Tokens.Jwt;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Services.Common;

public static class CommonExtensions
{
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        // Default health checks assume the event bus and self health checks
        builder.Services.AddDefaultHealthChecks(builder.Configuration);

        builder.Services.AddDefaultAuthentication(builder.Configuration);

        builder.Services.AddDefaultOpenApi(builder.Configuration);

        // Add the accessor
        builder.Services.AddHttpContextAccessor();

        return builder;
    }

    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

        var pathBase = app.Configuration["PATH_BASE"];

        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
            app.UseRouting();

            var identitySection = app.Configuration.GetSection("Identity");

            if (identitySection.Exists())
            {
                // We have to add the auth middleware to the pipeline here
                app.UseAuthentication();
                app.UseAuthorization();
            }
        }

        app.UseDefaultOpenApi(app.Configuration);

        app.MapDefaultHealthChecks();

        return app;
    }

    public static async Task<bool> CheckHealthAsync(this WebApplication app)
    {
        app.Logger.LogInformation("Running health checks...");

        // Do a health check on startup, this will throw an exception if any of the checks fail
        var report = await app.Services.GetRequiredService<HealthCheckService>().CheckHealthAsync();

        if (report.Status == HealthStatus.Unhealthy)
        {
            app.Logger.LogCritical("Health checks failed!");
            foreach (var entry in report.Entries)
            {
                if (entry.Value.Status == HealthStatus.Unhealthy)
                {
                    app.Logger.LogCritical("{Check}: {Status}", entry.Key, entry.Value.Status);
                }
            }

            return false;
        }

        return true;
    }

    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app, IConfiguration configuration)
    {
        var openApiSection = configuration.GetSection("OpenApi");

        if (!openApiSection.Exists())
        {
            return app;
        }

        //app.UseSwagger();
        //app.UseSwaggerUI(setup =>
        //{
        //    /// {
        //    ///   "OpenApi": {
        //    ///     "Endpoint: {
        //    ///         "Name": 
        //    ///     },
        //    ///     "Auth": {
        //    ///         "ClientId": ..,
        //    ///         "AppName": ..
        //    ///     }
        //    ///   }
        //    /// }

        //    var pathBase = configuration["PATH_BASE"];
        //    var authSection = openApiSection.GetSection("Auth");
        //    var endpointSection = openApiSection.GetRequiredSection("Endpoint");

        //    var swaggerUrl = endpointSection["Url"] ?? $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json";

        //    setup.SwaggerEndpoint(swaggerUrl, endpointSection.GetRequiredValue("Name"));

        //    if (authSection.Exists())
        //    {
        //        setup.OAuthClientId(authSection.GetRequiredValue("ClientId"));
        //        setup.OAuthAppName(authSection.GetRequiredValue("AppName"));
        //    }
        //});

        // Add a redirect from the root of the app to the swagger endpoint
        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

        return app;
    }

    public static IServiceCollection AddDefaultOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        var openApi = configuration.GetSection("OpenApi");

        if (!openApi.Exists())
        {
            return services;
        }

        services.AddEndpointsApiExplorer();

        return services;
        //return services.AddSwaggerGen(options =>
        //{
        //    /// {
        //    ///   "OpenApi": {
        //    ///     "Document": {
        //    ///         "Title": ..
        //    ///         "Version": ..
        //    ///         "Description": ..
        //    ///     }
        //    ///   }
        //    /// }
        //    var document = openApi.GetRequiredSection("Document");

        //    var version = document.GetRequiredValue("Version") ?? "v1";

        //    options.SwaggerDoc(version, new OpenApiInfo
        //    {
        //        Title = document.GetRequiredValue("Title"),
        //        Version = version,
        //        Description = document.GetRequiredValue("Description")
        //    });

        //    var identitySection = configuration.GetSection("Identity");

        //    if (!identitySection.Exists())
        //    {
        //        // No identity section, so no authentication open api definition
        //        return;
        //    }

        //    // {
        //    //   "Identity": {
        //    //     "ExternalUrl": "http://identity",
        //    //     "Scopes": {
        //    //         "basket": "Basket API"
        //    //      }
        //    //    }
        //    // }

        //    var identityUrlExternal = identitySection["ExternalUrl"] ?? identitySection.GetRequiredValue("Url");
        //    var scopes = identitySection.GetRequiredSection("Scopes").GetChildren().ToDictionary(p => p.Key, p => p.Value);

        //    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        //    {
        //        Type = SecuritySchemeType.OAuth2,
        //        Flows = new OpenApiOAuthFlows()
        //        {
        //            Implicit = new OpenApiOAuthFlow()
        //            {
        //                AuthorizationUrl = new Uri($"{identityUrlExternal}/connect/authorize"),
        //                TokenUrl = new Uri($"{identityUrlExternal}/connect/token"),
        //                Scopes = scopes,
        //            }
        //        }
        //    });

        //    options.OperationFilter<AuthorizeCheckOperationFilter>();
        //});
    }

    public static IServiceCollection AddDefaultAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // {
        //   "Identity": {
        //     "Url": "http://identity",
        //     "Audience": "basket"
        //    }
        // }

        var identitySection = configuration.GetSection("Identity");

        if (!identitySection.Exists())
        {
            // No identity section, so no authentication
            return services;
        }

        // prevent from mapping "sub" claim to nameidentifier.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        services.AddAuthentication().AddJwtBearer(options =>
        {
            var identityUrl = identitySection.GetRequiredValue("Url");
            var audience = identitySection.GetRequiredValue("Audience");

            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = audience;
            options.TokenValidationParameters.ValidateAudience = false;
        });

        return services;
    }

    public static IHealthChecksBuilder AddDefaultHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        // Health check for the application itself
        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        return hcBuilder;
    }

    public static void MapDefaultHealthChecks(this IEndpointRouteBuilder routes)
    {
        routes.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        routes.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });
    }
}
