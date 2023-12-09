using Basket.API;
using Basket.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.UseHttpClientMetrics();

builder.Services.AddGrpc();
builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddDbContexts(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck<BasketHealthCheck>(nameof(BasketHealthCheck))
    .ForwardToPrometheus();

builder.Services.AddTransient<IBasketRepository, BasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BasketContext>();
    db.Database.Migrate(); 
}

app.UseServiceDefaults();

app.UseHttpMetrics();

app.MapGrpcService<BasketService>();
app.MapControllers();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    // Enable the /metrics page to export Prometheus metrics.
    // Open http://localhost:5099/metrics to see the metrics.
    //
    // Metrics published in this sample:
    // * built-in process metrics giving basic information about the .NET runtime (enabled by default)
    // * metrics from .NET Event Counters (enabled by default, updated every 10 seconds)
    // * metrics from .NET Meters (enabled by default)
    // * metrics about requests made by registered HTTP clients used in SampleService (configured above)
    // * metrics about requests handled by the web app (configured above)
    // * ASP.NET health check statuses (configured above)
    // * custom business logic metrics published by the SampleService class
    endpoints.MapMetrics();
});

await app.RunAsync();
