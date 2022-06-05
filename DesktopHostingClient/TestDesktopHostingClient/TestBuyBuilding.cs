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
        // Get New GameData from API.
        GameDataService gameDataService = new GameDataService(_apiUrl);
        Task<GameData> gameDataTask = gameDataService.CreateGameData();
        gameDataTask.Wait();
        GameData gameData = gameDataTask.Result;

        // Get Purchasables from API.
        PurchasableService purchasableService = new PurchasableService(_apiUrl);
        Task<Dictionary<int, Purchasable>> purchasablesTask = purchasableService.GetPurchasables();
        purchasablesTask.Wait();

        // Insert GameData & Purchasables into GameManager instance.
        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData(gameData);
        gameManager.Purchasables = purchasablesTask.Result;

        // Try starting the host server.
        HostingManager hostingManager = TryStartHost();

        // Create SignalR Connection to the Host GameHub
        IHubConnectionBuilder hubConnectionBuilder = new HubConnectionBuilder();
        hubConnectionBuilder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = hubConnectionBuilder.Build();

        // Act 
        Dictionary<int, Purchasable> receivedPurchasables = null;

        connection.On<Dictionary<int, Purchasable>>("ReceivePurchasables", (purchasables) =>
        {
            receivedPurchasables = purchasables;
        });

        connection.StartAsync().Wait();

        // Wait for Host server to respond with "ReceivePurchasables"
        Thread.Sleep(500);

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
        // Create Test Purchasable and add it to a dictionary.
        Purchasable purchasable1 = new Purchasable()
        {
            Id = 1,
            Name = "TestPurchasable1",
            Price = purchasablePrice
        };
        Dictionary<int, Purchasable> purchasables = new Dictionary<int, Purchasable>();
        purchasables.Add(purchasable1.Id, purchasable1);

        // Create Test GameData.
        GameData gameData = new GameData()
        {
            Balance = startingBalance
        };

        // Add Purchasables and GameData to GameManager instance.
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
        // Create Test Purchasable and add it to a dictionary.
        Purchasable purchasable1 = new Purchasable()
        {
            Id = 1,
            Name = "TestPurchasable1",
            Price = 10
        };
        Dictionary<int, Purchasable> purchasables = new Dictionary<int, Purchasable>();
        purchasables.Add(purchasable1.Id, purchasable1);

        // Create Test GameData.
        GameData gameData = new GameData()
        {
            Balance = 15
        };

        // Add Purchasables and GameData to GameManager instance.
        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData(gameData);
        gameManager.Purchasables = purchasables;

        HostingManager hostingManager = TryStartHost();

        // Create SignalR Connection to the Host GameHub
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

        // Wait to receive "PurchaseUpdate" from 
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
        // Create Test Purchasable and add it to a dictionary.
        Purchasable purchasable1 = new Purchasable()
        {
            Id = 1,
            Name = "TestPurchasable1",
            Price = 10
        };
        Dictionary<int, Purchasable> purchasables = new Dictionary<int, Purchasable>();
        purchasables.Add(purchasable1.Id, purchasable1);

        // Create Test GameData.
        GameData gameData = new GameData()
        {
            Balance = 15
        };

        // Add Purchasables and GameData to GameManager instance.
        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData(gameData);
        gameManager.Purchasables = purchasables;

        HostingManager hostingManager = TryStartHost();

        // Create SignalR Connection to the Host GameHub
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

        // Wait to receive "PurchaseUpdate" from 
        Thread.Sleep(500);

        // Assert
        Assert.True(receivedPurchase);

        hostingManager.DisposeHost();
    }

    [Theory]
    [Trait("UserStory", "Buy Buildings")]
    [InlineData(20,7,2)]
    [InlineData(1,7,0)]
    [InlineData(1,1,1)]
    [InlineData(1,0,0)]
    [InlineData(1,-1,0)]
    public void BuyMaxPurchasables(int balance, int price, int expectedAmount)
    {
        // Arrange
        GameData gameData = new GameData()
        {
            Balance = balance
        };

        Purchasable purchasable = new Purchasable()
        {
            Id = 1,
            Price = price
        };
        Dictionary<int, Purchasable> purchasables = new Dictionary<int, Purchasable>();
        purchasables.Add(1, purchasable);

        GameManager gameManager = GameManager.GetInstance();
        gameManager.Purchasables = purchasables;
        gameManager.CreateGameData(gameData);

        // Act
        int purchasedAmount = gameManager.BuyMaxPurchasables(1);

        // Assert
        Assert.Equal(expectedAmount, purchasedAmount);
    }
}
