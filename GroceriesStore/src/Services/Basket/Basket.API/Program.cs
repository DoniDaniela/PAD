using Basket.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddDbContexts(builder.Configuration);

builder.Services.AddHealthChecks(builder.Configuration);

builder.Services.AddTransient<IBasketRepository, BasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BasketContext>();
    db.Database.Migrate();
}

app.UseServiceDefaults();

app.MapGrpcService<BasketService>();
app.MapControllers();

await app.RunAsync();
