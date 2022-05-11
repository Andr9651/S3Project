using DesktopHostingClient.Model;
using DesktopHostingClient.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestDesktopHostingClient
{
    public class TestGameDataService
    {
        [Fact]
        public void TestLoadGame()
        {
            //Arrange
            GameDataService gameDataService = new GameDataService();

            //Act
            Task<GameData> gameDataTask = gameDataService.LoadGameData(1);

            gameDataTask.Wait();

            GameData gameData = gameDataTask.Result;

            //Assert
            Assert.NotNull(gameData);
        }



    }
}
