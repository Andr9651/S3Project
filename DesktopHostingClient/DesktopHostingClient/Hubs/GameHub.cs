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

    public override async Task OnConnectedAsync()
    {
        GameManager.NotifyBalanceChanged();

        List<Purchasable> purchasables = GameManager.Purchasables.Values.ToList<Purchasable>();

        Clients.Caller.SendAsync("ReceivePurchasables", purchasables);

        Dictionary<int, int> readOnlyPurchases = (Dictionary<int, int>)GameManager.GetPurchases();

        Clients.Caller.SendAsync("ReceivePurchases", readOnlyPurchases);
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
}
