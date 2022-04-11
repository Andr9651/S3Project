using Microsoft.AspNetCore.SignalR.Client;

HubConnectionBuilder connectionBuilder = new HubConnectionBuilder();

connectionBuilder.WithUrl("https://localhost:5001/hub");

HubConnection connection = connectionBuilder.Build();

connection.StartAsync().Wait(); // ();

object[] parameters = new[] { "Candy Store" };

connection.InvokeCoreAsync("BuyBuilding", parameters);

Action<string> BuyBuildingResponse = (response) =>
{
    Console.WriteLine(response);
};

connection.On("BuyBuildingResponse", BuyBuildingResponse);

Console.ReadKey();



