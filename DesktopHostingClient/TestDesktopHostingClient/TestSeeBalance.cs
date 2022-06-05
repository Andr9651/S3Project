using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DesktopHostingClient.Managers;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;
using ModelLibrary.Model;

namespace TestDesktopHostingClient;

[Collection("Sequential")]
public class TestSeeBalance
{
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
    [Trait("UserStory", "See Balance")]
    public void TestReciveBalanceUpdate()
    {
        // Arrange 
        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData();
        HostingManager hostingManager = TryStartHost();

        IHubConnectionBuilder hubConnectionBuilder = new HubConnectionBuilder();
        hubConnectionBuilder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = hubConnectionBuilder.Build();

        // Act 

        bool receivedUpdate = false;
        connection.On<int>("BalanceUpdate", (balance) =>
        {
            receivedUpdate = true;
        });
        connection.StartAsync().Wait();
        gameManager.NotifyBalanceChanged();
        Thread.Sleep(500);

        //Assert
        Assert.True(receivedUpdate);

        connection.DisposeAsync().AsTask().Wait();

        hostingManager.DisposeHost();
    }

    [Theory]
    [Trait("UserStory", "Add Income Increase")]
    [InlineData(1,1,2)]
    [InlineData(1,5,6)]
    [InlineData(2,5,11)]
    public void IncreaseIncomeWithMorePurchases(
        int purchasableIncome,
        int purchaseCount,
        int expectedIncomePerSecond)
    {
        // Arrange

        Purchasable purchasable = new Purchasable()
        {
            Id = 1,
            Income = purchasableIncome
        };
        Dictionary<int, Purchasable> purchasables = new Dictionary<int, Purchasable>();
        purchasables.Add(1, purchasable);

        GameData gameData = new GameData();
        gameData.Purchases.Add(1, purchaseCount);

        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData(gameData);
        gameManager.Purchasables = purchasables;

        // Act
        int incomePerSecond = gameManager.IncomePerSecond;

        // Assert
        Assert.Equal(expectedIncomePerSecond, incomePerSecond);

    }
}
