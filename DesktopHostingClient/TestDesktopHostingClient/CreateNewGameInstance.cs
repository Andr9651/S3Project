using Xunit;
using DesktopHostingClient.Controller;
using DesktopHostingClient;
using System;

namespace TestDesktopHostingClient;

public class CreateNewGameInstance
{
    [Fact]
    public void TestCreateGame()
    {
        //Arrange
        GameDataController gameDataController = new GameDataController();

        //Act
        gameDataController.CreateGameData();
        
        //Assert
        Assert.NotNull(gameDataController.GameData);
    }
}
