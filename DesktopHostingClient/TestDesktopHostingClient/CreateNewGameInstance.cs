using Xunit;
using DesktopHostingClient.Managers;
using DesktopHostingClient;
using System;

namespace TestDesktopHostingClient;

public class CreateNewGameInstance
{
    [Fact]
    public void TestCreateGame()
    {
        //Arrange
        GameDataManager gameDataManager =  GameDataManager.GetInstance();

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
}
