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

    public GameData? GetCurrentGameData()
    {
        return GameDataManager.GameData;
    } 

    public void Ping()
    {
        Clients.Caller.SendAsync("Pong");
    }

}
