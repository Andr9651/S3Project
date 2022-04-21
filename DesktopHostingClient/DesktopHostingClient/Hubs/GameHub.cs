using DesktopHostingClient.Controller;
using Microsoft.AspNetCore.SignalR;

namespace DesktopHostingClient.Hubs;


public class GameHub : Hub
{
    public GameDataController GameDataController { get; set; }

    public GameHub()
    {
            GameDataController =  GameDataController.GetInstance();

    }



}
