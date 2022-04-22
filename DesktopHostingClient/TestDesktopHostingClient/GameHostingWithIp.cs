using DesktopHostingClient.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.SignalR.Client;
using DesktopHostingClient.Model;

namespace TestDesktopHostingClient;

public class GameHostingWithIp
{
    [Fact]
    public void TestGetSignalRConnection()
    {
        //Arrange 
        HostingManager hostingManager = new HostingManager();
        //Act
        hostingManager.SetupSignalRHost();

        hostingManager.StartHosting().Wait();

        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = builder.Build();
        connection.StartAsync().Wait();


        //Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);

        hostingManager.DisposeHost();
        connection.DisposeAsync();
    }

    [Fact]
    public void TestGetGameReference()
    {
        //Arrange
        GameDataManager gameDataManager = GameDataManager.GetInstance();
        HostingManager hostingManager = new HostingManager();
        
        //Act
        hostingManager.SetupSignalRHost();

        hostingManager.StartHosting().Wait();

        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = builder.Build();
        connection.StartAsync().Wait();
        gameDataManager.CreateGameData();

        Task<GameData> gameDataTask = connection.InvokeAsync<GameData>("GetCurrentGameData");
        gameDataTask.Wait();
        GameData gameData = gameDataTask.Result;

        //Assert
        Assert.NotNull(gameData);

        hostingManager.DisposeHost();
        connection.DisposeAsync();
    }

}
