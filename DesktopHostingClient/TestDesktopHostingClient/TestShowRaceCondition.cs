using DesktopHostingClient.Managers;
using ModelLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestDesktopHostingClient;

[Collection("Sequential")]
public class TestShowRaceCondition
{
    [Theory]
    [Trait("UserStory", "Show Race Condition")]
    [Trait("UserStory", "Buy Buildings")]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(1000000)]
    //[InlineData(100000000)]
    public void StressTestTryBuyPurchasable(int repetitions)
    {
        //Arrange
        GameManager gameManager = GameManager.GetInstance();
        gameManager.Purchasables = new Dictionary<int, Purchasable>();
        
        Purchasable purchasable = new Purchasable()
        {
            Price = 5
        };
        gameManager.Purchasables.Add(1, purchasable);

        GameData gameData = new GameData()
        {
            Balance = 5 * repetitions // Purchasable.price * repetitions
        };
        gameManager.CreateGameData(gameData);

        //Act
        Parallel.For(0, repetitions, (i) =>
        {
            gameManager.TryBuyPurchasable(1);
        });

        //Arrange
        Assert.Equal(0, gameManager.GetBalance());
        Assert.Equal(repetitions, gameManager.GetPurchasedAmount(1));
        //Assert.True(gameManager.GetBalance() == 0,$"Balance was not nill, Actual: {gameManager.GetBalance()} ");

    }
}
