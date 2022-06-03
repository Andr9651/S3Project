using DesktopHostingClient.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;


namespace DesktopHostingClient.Managers;
public class HostingManager
{
    public string Port { get; set; }
    private IHost _host;
    // Represents the connections in the SignalR GameHub
    private IHubContext<GameHub>? _gameHubConnections;

    public HostingManager(string? port = null)
    {
        if(port is null)
        {
        Port = ConfigurationManager.ConnectionStrings["HostPort"].ToString() ;
        }
        else
        {
            Port = port;
        }

        // Subscribe actions to GameManager events
        GameManager gameManager = GameManager.GetInstance();
        gameManager.OnBalanceChanged += PushBalanceToClients;
        gameManager.OnPurchase += PushPurchaseToClients;
    }

    private void PushPurchaseToClients(int purchaseId, int amount)
    {
        // Sends a PurchaseUpdate to all clients
        _gameHubConnections?.Clients.All.SendAsync("PurchaseUpdate", purchaseId, amount);
    }

    private void PushBalanceToClients(int balance)
    {
        // Sends a BalanceUpdate to all clients
        _gameHubConnections?.Clients.All.SendAsync("BalanceUpdate", balance);
    }

    public void SetupSignalRHost()
    {
        DisposeHost();

        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        // Configures the ServiceCollection
        Action<IServiceCollection> serviceConfiguration = (services) =>
        {
            // adds SignalR services to the ServiceCollection including "IHubContext<GameHub>"
            services.AddSignalR();

            // Add a Cross-Origin Resource Sharing (Cors) policy that allows anything.
            services.AddCors((services) =>
            {
                services.AddPolicy("AllowAny", (policy) =>
                {
                    policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });
        };

        // Configures the applicationBuilder.
        Action<IApplicationBuilder> applicationConfiguration = (app) =>
        {
            // Use the created policy.
            app.UseCors("AllowAny");

            // Adds middleware that routes requests to endpoints.
            app.UseRouting();

            // Creates an endpoint to access the SignalR hub "Gamehub".
            app.UseEndpoints((endpoints) => endpoints.MapHub<GameHub>("/GameHub"));
        };
        
        // Configures The WebHostBuilder with the previous configurations.
        Action<IWebHostBuilder> webHostConfiguration = (webBuilder) =>
        {
            webBuilder.UseUrls($"http://localhost:{Port}");
            webBuilder.ConfigureServices(serviceConfiguration);
            webBuilder.Configure(applicationConfiguration);
        };

        hostBuilder.ConfigureWebHostDefaults(webHostConfiguration);

        _host = hostBuilder.Build();
    }

    public async Task StartHosting()
    {
        await _host.StartAsync();

        // Gets the GameHubContext from the hosts service collection.
        _gameHubConnections = _host.Services.GetService<IHubContext<GameHub>>();
    }

    public void DisposeHost()
    {
        // The ? checks if host is null if its not null it will dispose the host
        _host?.Dispose();
    }

    // The public ip can't be seen from inside the computer, only from the outside.
    // We send an API call to ipify, who then responds with our public ip.
    public async Task<string> GetPublicIp()
    {
        HttpClient client = new HttpClient();

        string ip = await client.GetStringAsync("http://api.ipify.org");

        return ip;
    }
}
