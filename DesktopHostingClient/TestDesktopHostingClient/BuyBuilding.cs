using DesktopHostingClient.Managers;
using DesktopHostingClient.Model;
using DesktopHostingClient.Service;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TestDesktopHostingClient;

public class BuyBuilding
{

    [Fact]
    public void TestPurchasableService()
    {

        //Arrange 
        PurchasableService purchasableService = new PurchasableService();

        //Act 
        Task<List<Purchasable>> task = purchasableService.GetPurchasables();
        task.Wait();
        List<Purchasable> foundPurchasable = task.Result;
        
        //Assert
        Assert.NotNull(foundPurchasable);

    }

    [Fact]
    public void TestReceivePurchasables()
    {

        // Arrange 
        GameDataManager gameDataManager = GameDataManager.GetInstance();

        gameDataManager.HostingStartUp();
        HostingManager hostingManager = new HostingManager();
        hostingManager.SetupSignalRHost();
        hostingManager.StartHosting().Wait();

        // Act 
        IHubConnectionBuilder hubConnectionBuilder = new HubConnectionBuilder();
        hubConnectionBuilder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = hubConnectionBuilder.Build();
        List<Purchasable> receivedPurchasables = null;
        connection.On<List<Purchasable>>("ReceivePurchasables", (purchasables) =>
        {
            receivedPurchasables = purchasables;
        });
        connection.StartAsync().Wait();
        Thread.Sleep(2000);

        //Assert
        Assert.NotNull(receivedPurchasables);
        Assert.True(receivedPurchasables.Count > 0);
    }
}
