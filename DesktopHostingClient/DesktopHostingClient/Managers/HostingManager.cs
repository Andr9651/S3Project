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

namespace DesktopHostingClient.Managers;

public class HostingManager
{
    private IHost _host;
    private string _port;
    

    public string Port
    {
        get { return _port; }
        set { _port = value; }
    }

    public HostingManager()
    {
        _port = "5100";
    }

    public void SetupSignalRHost()
    {
        DisposeHost();

        IHostBuilder hostBuilder = Host.CreateDefaultBuilder();

        Action<IServiceCollection> serviceCollection = services =>
        {
            services.AddSignalR();
        };

        Action<IApplicationBuilder> applicationBuilder = app =>
        {
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


        Console.WriteLine(_host);
    }
    public async Task StartHosting()
    {
        await _host.StartAsync();
    }

    public void DisposeHost()
    {
        // Check if host is null if its not null it will dispose the host
        _host?.Dispose();

    }

    public async Task<string> GetPublicIP()
    {
        HttpClient client = new HttpClient();

        string ip = await client.GetStringAsync("http://api.ipify.org");
        
        return ip; 
    }

}
