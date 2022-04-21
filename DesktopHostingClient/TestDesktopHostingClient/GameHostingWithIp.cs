using DesktopHostingClient.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.SignalR.Client;

namespace TestDesktopHostingClient;

public class GameHostingWithIp
{
    [Fact]
    public void TestGetSignalRConnection()
    {
        //Arrange 
        HostingController hostingController = new HostingController();
        //Act
        hostingController.SetupSignalRHost();

        hostingController.StartHosting().Wait();

        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = builder.Build();
        connection.StartAsync().Wait();


        //Assert
        Assert.Equal(HubConnectionState.Connected, connection.State);


    }
    public void TestGetGameRefrence()
    {
        //Arrange
        GameDataController gameDataController = GameDataController.GetInstance();
        HostingController hostingController = new HostingController();
        
        //Act
        hostingController.SetupSignalRHost();

        hostingController.StartHosting().Wait();

        HubConnectionBuilder builder = new HubConnectionBuilder();
        builder.WithUrl("http://localhost:5100/GameHub");
        HubConnection connection = builder.Build();
        connection.StartAsync().Wait();
        gameDataController.CreateGameData();


        //Assert
    }

}
