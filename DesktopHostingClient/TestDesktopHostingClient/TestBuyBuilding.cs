using DesktopHostingClient.Managers;
using ModelLibrary.Model;
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

[Collection("Sequential")]
public class TestBuyBuilding
{
    private string _apiUrl = "https://localhost:7236";
    private string _hostPort = "5100";

    private HostingManager TryStartHost()
    {
        HostingManager hostingManager = new HostingManager(_hostPort);

        for (int i = 0; i < 10; i++)
        {
            try
            {
                hostingManager.SetupSignalRHost();
                hostingManager.StartHosting().Wait();

                break;
            }
            catch (Exception)
            {
                Thread.Sleep(500);
            }
        }

        return hostingManager;
    }

    [Fact]
    [Trait("UserStory","Buy Buildings")]
    public void TestPurchasableService()
    {
        //Arrange 
        PurchasableService purchasableService = new PurchasableService(_apiUrl);

        //Act 
        Task<Dictionary<int, Purchasable>> task = purchasableService.GetPurchasables();
        task.Wait();
        Dictionary<int, Purchasable> foundPurchasable = task.Result;

        //Assert
        Assert.NotNull(foundPurchasable);
    }

    [Fact]
    [Trait("UserStory", "Buy Buildings")]
    public void TestReceivePurchasables()
    {
        // Arrange 
        GameManager gameManager = GameManager.GetInstance();

        GameDataService gameDataService = new GameDataService(_apiUrl);

        Task<GameData> gameDataTask = gameDataService.CreateGameData();
        gameDataTask.Wait();

        GameData gameData = gameDataTask.Result;

        gameManager.CreateGameData(gameData);

        PurchasableService purchasableService = new PurchasableService(_apiUrl);

        Task<Dictionary<int, Purchasable>> purchasablesTask = purchasableService.GetPurchasables();
        purchasablesTask.Wait();

        gameManager.Purchasables = purchasablesTask.Result;

        //gameManager.SetupGame().Wait();

        HostingManager hostingManager = TryStartHost();

        // Act 
        IHubConnectionBuilder hubConnectionBuilder = new HubConnectionBuilder();
        hubConnectionBuilder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = hubConnectionBuilder.Build();

        Dictionary<int, Purchasable> receivedPurchasables = null;

        connection.On<Dictionary<int, Purchasable>>("ReceivePurchasables", (purchasables) =>
        {
            receivedPurchasables = purchasables;
        });

        connection.StartAsync().Wait();

        Thread.Sleep(2000);

        //Assert
        Assert.NotNull(receivedPurchasables);
        Assert.True(receivedPurchasables.Count > 0);

        hostingManager.DisposeHost();
    }

    [Theory]
    [Trait("UserStory", "Buy Buildings")]
    [InlineData(10, 5, 1, true)]
    [InlineData(10, 15, 1, false)]
    [InlineData(10, 5, 2, false)]
    [InlineData(10, 10, 1, true)]
    // We allow purchasables with a negative price
    [InlineData(10, -1, 1, true)]
    public void TestBuy(int startingBalance, int purchasablePrice, int buyId, bool shouldSucceed)
    {
        // Arrange


        Purchasable purchasable1 = new Purchasable()
        {
            Id = 1,
            Name = "TestPurchasable1",
            Price = purchasablePrice
        };

        Dictionary<int, Purchasable> purchasables = new Dictionary<int, Purchasable>();
        purchasables.Add(purchasable1.Id, purchasable1);

        GameData gameData = new GameData()
        {
            Balance = startingBalance
        };

        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData(gameData);
        gameManager.Purchasables = purchasables;

        // Act
        bool isSuccess = gameManager.TryBuyPurchasable(buyId);
        // Assert
        if (shouldSucceed)
        {
            Assert.Equal(startingBalance - purchasablePrice, gameManager.GetBalance());
            Assert.Equal(1, gameManager.GetPurchasedAmount(buyId));
        }
        else
        {
            Assert.Equal(startingBalance, gameManager.GetBalance());
            Assert.Equal(0, gameManager.GetPurchasedAmount(buyId));
        }

        Assert.Equal(shouldSucceed, isSuccess);
    }

    [Fact]
    [Trait("UserStory", "Buy Buildings")]
    public void ReceivePurchaseUpdate()
    {
        // Arrange
        Purchasable purchasable1 = new Purchasable()
        {
            Id = 1,
            Name = "TestPurchasable1",
            Price = 10
        };

        Dictionary<int, Purchasable> purchasables = new Dictionary<int, Purchasable>();
        purchasables.Add(purchasable1.Id, purchasable1);

        GameData gameData = new GameData()
        {
            Balance = 15
        };

        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData(gameData);
        gameManager.Purchasables = purchasables;

        HostingManager hostingManager = TryStartHost();

        HubConnectionBuilder hubConnectionBuilder = new HubConnectionBuilder();
        hubConnectionBuilder.WithUrl("http://localhost:5100/GameHub");
        HubConnection hubConnection = hubConnectionBuilder.Build();

        // Arrange
        bool receivedPurchase = false;

        hubConnection.On<int, int>("PurchaseUpdate", (purchaseId, amount) =>
        {
            receivedPurchase = true;
        });
        hubConnection.StartAsync().Wait();

        gameManager.TryBuyPurchasable(1);
        Thread.Sleep(500);

        // Assert
        Assert.True(receivedPurchase);

        hostingManager.DisposeHost();
    }

    [Fact]
    [Trait("UserStory","Buy Buildings")]
    public void TryBuyPurchasableRPC()
    {
        // Arrange
        Purchasable purchasable1 = new Purchasable()
        {
            Id = 1,
            Name = "TestPurchasable1",
            Price = 10
        };

        Dictionary<int, Purchasable> purchasables = new Dictionary<int, Purchasable>();
        purchasables.Add(purchasable1.Id, purchasable1);

        GameData gameData = new GameData()
        {
            Balance = 15
        };

        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData(gameData);
        gameManager.Purchasables = purchasables;

        HostingManager hostingManager = TryStartHost();

        HubConnectionBuilder hubConnectionBuilder = new HubConnectionBuilder();
        hubConnectionBuilder.WithUrl("http://localhost:5100/GameHub");
        HubConnection hubConnection = hubConnectionBuilder.Build();

        // Act
        bool receivedPurchase = false;

        hubConnection.On<int, int>("PurchaseUpdate", (purchaseId, amount) =>
        {
            receivedPurchase = true;
        });
        hubConnection.StartAsync().Wait();

        hubConnection.InvokeAsync("TryBuyPurchasable", 1);
        Thread.Sleep(500);

        // Assert
        Assert.True(receivedPurchase);

        hostingManager.DisposeHost();
    }
}
