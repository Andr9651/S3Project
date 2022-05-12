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

[Collection("Sequential")]
public class TestSeeBalance
{
    private HostingManager TryStartHost()
    {
        HostingManager hostingManager = new HostingManager();

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
        gameManager.NotifyBalanceChanged();
        Thread.Sleep(500);

        //Assert
        Assert.True(receivedUpdate);

        connection.DisposeAsync().AsTask().Wait();

        hostingManager.DisposeHost();
    }
}
