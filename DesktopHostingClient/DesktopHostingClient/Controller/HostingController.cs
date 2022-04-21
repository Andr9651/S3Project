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

namespace DesktopHostingClient.Controller;

public class HostingController
{
    private IHost _host;

    public void SetupSignalRHost()
    {
        // Check if host is null if its not null it will dispose the host
        _host?.Dispose();

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
            webBuilder.UseUrls("http://localhost:5100");
            webBuilder.ConfigureServices(serviceCollection);
            webBuilder.Configure(applicationBuilder);
        };

        hostBuilder.ConfigureWebHostDefaults(webHostBuilder);

        _host = hostBuilder.Build();

        
        Console.WriteLine(_host);
    }
    public async     Task
StartHosting()
    {
        await _host.StartAsync();
    }
}
