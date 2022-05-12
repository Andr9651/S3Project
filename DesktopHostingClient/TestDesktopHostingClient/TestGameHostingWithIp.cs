using DesktopHostingClient.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.SignalR.Client;
using DesktopHostingClient.Model;
using System.Threading;

namespace TestDesktopHostingClient;

[Collection("Sequential")]
public class TestGameHostingWithIp : IDisposable
{
    private HostingManager _hostingManager;
    public TestGameHostingWithIp()
    {
        for (int i = 0; i < 10; i++)
        {

            try
            {
                _hostingManager = new HostingManager();
                _hostingManager.SetupSignalRHost();
                _hostingManager.StartHosting().Wait();

                break;
            }
            catch (Exception)
            {
                Thread.Sleep(500);
            }
        }
    }

    [Fact]
    public void TestGetSignalRConnection()
    {
        //Arrange 

        //Act
        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = builder.Build();
        connection.StartAsync().Wait();


        //Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);

        connection.DisposeAsync().AsTask().Wait();
    }

    [Fact]
    public void TestGetGameReference()
    {
        //Arrange
        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData();

        //Act
        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = builder.Build();
        connection.StartAsync().Wait();

        Task<bool> gameDataTask = connection.InvokeAsync<bool>("HasGame");
        gameDataTask.Wait();
        bool gameData = gameDataTask.Result;

        //Assert
        Assert.True(gameData);

        connection.DisposeAsync().AsTask().Wait();
    }

    [Fact]
    public void RecieveRPCfromHost()
    {
        //Arrange
        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData();

        //Act
        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = builder.Build();

        bool ponged = false;

        connection.On("Pong", () =>
        {
            ponged = true;
        });

        connection.StartAsync().Wait();
        connection.InvokeAsync("Ping").Wait();

        Thread.Sleep(500);

        //Assert
        Assert.True(ponged);

        connection.DisposeAsync().AsTask().Wait();
    }

    public void Dispose()
    {
        _hostingManager.DisposeHost();
    }

}
