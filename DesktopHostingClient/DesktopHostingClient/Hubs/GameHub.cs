using DesktopHostingClient.Managers;
using ModelLibrary.Model;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace DesktopHostingClient.Hubs;
public class GameHub : Hub
{
    public GameManager GameManager { get; set; }

    public GameHub()
    {
        GameManager = GameManager.GetInstance();
    }

    // Gets executed when a client connects to the hub.
    public override async Task OnConnectedAsync()
    {
        // NotifyBalanceChanged updates all clients, which is not needed.
        // GameManager.NotifyBalanceChanged();

        // Clients.Caller only updates the caller.
        await Clients.Caller.SendAsync("BalanceUpdate", GameManager.GetBalance());

        Dictionary<int, Purchasable> purchasables = GameManager.Purchasables;
        await Clients.Caller.SendAsync("ReceivePurchasables", purchasables);

        Dictionary<int, int> purchases = GameManager.GetPurchases();
        await Clients.Caller.SendAsync("ReceivePurchases", purchases);
    }

    public bool HasGame()
    {
        return GameManager.HasGameData;
    }

    public void Ping()
    {
        Clients.Caller.SendAsync("Pong");
    }

    public bool TryBuyPurchasable(int purchasableId)
    {
        return GameManager.TryBuyPurchasable(purchasableId);
    }

    public int BuyMaxPurchasable(int purchasableId)
    {
        return GameManager.BuyMaxPurchasable(purchasableId);
    }

    public void SendMessage(string message, string user)
    {
        Clients.All.SendAsync("ReceiveMessage", message, user);
    }
}
