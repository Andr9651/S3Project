using Microsoft.AspNetCore.SignalR;


namespace Spike_SignalR;
public class ShopHub : Hub
{

    public async Task<string> BuyBuilding(String building)
    {
        //Check if we can buy the building

        //buy the building and change the game state to reflect that
        string response = $"The {building} was bought";


        Console.WriteLine($"=====================\nUser tried to buy a {building}\n==================");

        //announce to all players how the game state has changed

        await Clients.All.SendAsync("BuyBuildingResponse", response);

        return response;
    }

}