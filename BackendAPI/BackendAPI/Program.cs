using BackendAPI.API;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder.Build();

// Redirects HTTP requests to HTTPS instead
app.UseHttpsRedirection();

Endpoints.SetupEndpoints(app);

app.Run();
