using BackendAPI.API;
using BackendAPI.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SQLGameDataService>();

WebApplication app = builder.Build();

// Redirects HTTP requests to HTTPS instead
app.UseHttpsRedirection();

app.SetupEndpoints();

app.Run();
