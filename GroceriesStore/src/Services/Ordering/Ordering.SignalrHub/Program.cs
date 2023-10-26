var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSignalR(builder.Configuration);

var app = builder.Build();

app.UseServiceDefaults();

app.MapHub<NotificationsHub>("/hub/notificationhub");

await app.RunAsync();
