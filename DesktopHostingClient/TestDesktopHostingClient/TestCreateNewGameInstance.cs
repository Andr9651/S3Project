using Xunit;
using DesktopHostingClient.Managers;
using DesktopHostingClient;
using System;
using System.Threading;

namespace TestDesktopHostingClient;

public class TestCreateNewGameInstance
{
    [Fact]
    public void TestCreateGame()
    {
        //Arrange
        GameManager gameManager = GameManager.GetInstance();

        //Act
        gameManager.CreateGameData();

        //Assert
        Assert.True(gameManager.HasGameData);
    }
    [Fact]
    public void TestGetBalance()
    {
        //Arrange
        GameManager gameManager = GameManager.GetInstance();

        //Act
        gameManager.CreateGameData();

        //Assert
        Assert.Equal(0, gameManager.GetBalance());
    }
    [Fact]
    public void TestStartBalanceUpdateThread()
    {
        //Arrange 
        GameManager gameManager = GameManager.GetInstance();
        gameManager.CreateGameData();

        //Act 
        gameManager.StartBalanceUpdateThread();
        Thread.Sleep(1002);

        gameManager.StopBalanceUpdateThread();
        int balance = gameManager.GetBalance();
        Thread.Sleep(1002);

        //Assert
        Assert.True(gameManager.GetBalance() > 0);
        Assert.False(gameManager.IsUpdateThreadRunning);
        Assert.Equal(balance, gameManager.GetBalance());
    }
}
