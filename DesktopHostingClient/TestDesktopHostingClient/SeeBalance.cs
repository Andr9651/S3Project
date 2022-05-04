using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DesktopHostingClient.Managers;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;
namespace TestDesktopHostingClient;

public class SeeBalance
{
    [Fact]
    public void TestReciveBalanceUpdate()
    {
        // Arrange 
        GameDataManager gameDataManager = GameDataManager.GetInstance();
        gameDataManager.CreateGameData();
        HostingManager hostingManager = new HostingManager();
        hostingManager.SetupSignalRHost();
        hostingManager.StartHosting().Wait();

        // Act 
        IHubConnectionBuilder hubConnectionBuilder = new HubConnectionBuilder();
        hubConnectionBuilder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = hubConnectionBuilder.Build();
        bool receivedUpdate = false;
        connection.On<int>("BalanceUpdate", (balance) =>
        {
            receivedUpdate = true;
        });
        connection.StartAsync().Wait();
        gameDataManager.NotifyBalanceChanged();
        Thread.Sleep(500);

        //Assert
        Assert.True(receivedUpdate);
    }
}
