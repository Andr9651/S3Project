using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DesktopHostingClient.Managers;
using Microsoft.AspNetCore.SignalR.Client;
 
namespace TestDesktopHostingClient;

public class SeeBalance
{

    public void HandleBalanceUpdate (int balance)
    {
        console

    }





    [Fact]

    public void TestReciveBalanceUpdate ()
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
        bool reciveUpdate = false;
        //connection.On("BalanceUpdate", (balance) =>
        //{
        //    reciveUpdate = true;
        //});

        // Assert

        



    }





}
