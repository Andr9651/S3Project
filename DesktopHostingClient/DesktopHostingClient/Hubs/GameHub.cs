using DesktopHostingClient.Managers;
using DesktopHostingClient.Model;
using Microsoft.AspNetCore.SignalR;

namespace DesktopHostingClient.Hubs;


public class GameHub : Hub
{
    public GameDataManager GameDataManager { get; set; }

    public GameHub()
    {
            GameDataManager =  GameDataManager.GetInstance();

    }

    public bool HasGame()
    {
        return GameDataManager.HasGameData;
    } 

    public void Ping()
    {
        Clients.Caller.SendAsync("Pong");
    }

}
