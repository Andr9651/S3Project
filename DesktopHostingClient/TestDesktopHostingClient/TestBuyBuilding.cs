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

public class TestBuyBuilding
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
        GameManager gameManager = GameManager.GetInstance();

        gameManager.SetupGame();
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

    [Theory]
    [InlineData(10,5,1,true)]
    [InlineData(10,15,1,false)]
    public void TestBuy(int startingBalance, int purchasablePrice, int buyId, bool shouldSucceed)
    {
        // Arrnge
        GameManager gameManager = GameManager.GetInstance();
        GameData gameData = new GameData() { 
                Balance = startingBalance 
        };
        gameManager.CreateGameData(gameData);

        Dictionary<int,Purchasable> purchasables = new Dictionary<int,Purchasable>();
        Purchasable purchasable1 = new Purchasable() { 
                Id = 1, 
                Name = "TestPurchasable1", 
                Price = purchasablePrice 
        };

        purchasables.Add(purchasable1.Id, purchasable1);
        gameManager.Purchasables = purchasables;

        // Act
        bool isSuccess = gameManager.TryBuyBuilding(buyId);

        // Assert
        Assert.Equal(shouldSucceed, isSuccess);
        if (shouldSucceed)
        {
            Assert.Equal(startingBalance - purchasablePrice, gameManager.GetBalance());
        }
        else
        {
            Assert.Equal(startingBalance, gameManager.GetBalance());
        }

    }
}
