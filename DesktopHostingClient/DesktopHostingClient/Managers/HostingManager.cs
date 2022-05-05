using DesktopHostingClient.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.Certificate;

namespace DesktopHostingClient.Managers;
public class HostingManager
{
    public string Port
    {
        get { return _port; }
        set { _port = value; }
    }

    private IHost _host;
    private string _port;
    private IHubContext<GameHub> _hubContext;

    public HostingManager()
    {
        _port = "5100";

        GameManager gameManager = GameManager.GetInstance();
        gameManager.OnBalanceChanged += PushBalanceToClient;
    }

    public void PushBalanceToClient(int balance)
    {
        _hubContext.Clients.All.SendAsync("BalanceUpdate", balance);
    }

    public void SetupSignalRHost()
    {
        DisposeHost();

        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        Action<IServiceCollection> serviceCollection = services =>
        {
            services.AddSignalR();
            services.AddCors(services =>
            {
                services.AddPolicy("test", (policy) =>
                {
                    policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                });
            });
        };

        Action<IApplicationBuilder> applicationBuilder = app =>
        {
            app.UseCors("test");
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapHub<GameHub>("/GameHub"));
        };

        Action<IWebHostBuilder> webHostBuilder = webBuilder =>
        {
            webBuilder.UseUrls($"http://localhost:{_port}");
            webBuilder.ConfigureServices(serviceCollection);
            webBuilder.Configure(applicationBuilder);
        };

        hostBuilder.ConfigureWebHostDefaults(webHostBuilder);

        _host = hostBuilder.Build();
    }

    public async Task StartHosting()
    {
        await _host.StartAsync();

        GameManager gameDataManager =  GameManager.GetInstance();

        _hubContext = (IHubContext<GameHub>)_host.Services.GetService(typeof(IHubContext<GameHub>));
    }

    public void DisposeHost()
    {
        // The ? checks if host is null if its not null it will dispose the host
        _host?.Dispose();
    }

    public async Task<string> GetPublicIp()
    {
        HttpClient client = new HttpClient();

        string ip = await client.GetStringAsync("http://api.ipify.org");

        return ip;
    }
}
