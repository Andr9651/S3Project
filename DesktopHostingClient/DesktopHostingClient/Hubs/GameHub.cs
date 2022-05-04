using DesktopHostingClient.Managers;
using DesktopHostingClient.Model;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

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

    public override async Task OnConnectedAsync ()
    {
        GameDataManager.NotifyBalanceChanged();
        List<Purchasable> purchasables = GameDataManager.Purchasables.Values.ToList<Purchasable>();
        Clients.Caller.SendAsync("ReceivePurchasables", purchasables);
    }
}
