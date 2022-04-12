using Microsoft.AspNetCore.SignalR;
using Spike_SignalR.BackgroundServices;

namespace Spike_SignalR;
public class ShopHub : Hub
{

    public async Task<string> BuyBuilding(String building, string? connectionId)
    {
        //Check if we can buy the building

        //buy the building and change the game state to reflect that
        string response = $"The {building} was bought";

        BalanceUpdater.balance -= 600;

        Console.WriteLine($"=====================\nUser tried to buy a {building}\n==================");

        //announce to all players how the game state has changed

        await Clients.Caller.SendAsync("BuyBuildingResponse", response + "U da bes");

        /*
        await Clients.AllExcept(Clients.Calle)
            .SendAsync("BuyBuildingResponse", response);
        */

        return response;
    }

}