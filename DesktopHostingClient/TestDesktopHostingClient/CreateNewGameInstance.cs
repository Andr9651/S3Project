using Xunit;
using DesktopHostingClient.Managers;
using DesktopHostingClient;
using System;
using System.Threading;

namespace TestDesktopHostingClient;

public class CreateNewGameInstance
{
    [Fact]
    public void TestCreateGame()
    {
        //Arrange
        GameDataManager gameDataManager = GameDataManager.GetInstance();

        //Act
        gameDataManager.CreateGameData();

        //Assert
        Assert.True(gameDataManager.HasGameData);
    }
    [Fact]
    public void TestGetBalance()
    {
        //Arrange
        GameDataManager gameDataManager = GameDataManager.GetInstance();

        //Act
        gameDataManager.CreateGameData();

        //Assert
        Assert.Equal(0, gameDataManager.GetBalance());
    }
    [Fact]
    public void TestStartBalanceUpdateThread()
    {
        //Arrange 
        GameDataManager gameDataManager = GameDataManager.GetInstance();
        gameDataManager.CreateGameData();

        //Act 
        gameDataManager.StartBalanceUpdateThread();
        Thread.Sleep(1002);

        gameDataManager.StopBalanceUpdateThread();
        int balance = gameDataManager.GetBalance();
        Thread.Sleep(1002);

        //Assert
        Assert.True(gameDataManager.GetBalance() > 0);
        Assert.False(gameDataManager.IsUpdateThreadRunning);
        Assert.Equal(balance, gameDataManager.GetBalance());
    }
}
