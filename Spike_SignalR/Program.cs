using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.SignalR;

namespace Spike_SignalR;
public class Program
{
    public Program()
    {

    }

    static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSignalR();

        WebApplication app = builder.Build();        

        app.MapHub<ShopHub>("/hub");

        app.Run();
    }
}
