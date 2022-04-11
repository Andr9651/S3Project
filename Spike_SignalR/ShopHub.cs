using Microsoft.AspNetCore.SignalR;


namespace Spike_SignalR;
public class ShopHub : Hub
{

    public async Task BuyBuilding(String building)
    {
        //Check if we can buy the building

        //buy the building and change the game state to reflect that


        //announce to all players how the game state has changed
        await Clients.All.SendAsync("BuyBuildingResponse", $"The {building} was bought");
    }

}