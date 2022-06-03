using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using WebApp.Managers;
using System.Threading;

namespace TestWebApp;

public class SeeBalance
{
    [Fact]
    [Trait("UserStory", "See Balance")]
    public void TestGetBalanceValue()
    {
        //Arrange 
        GameManager gameManager = new GameManager();

        //Act 
        gameManager.ConnectToGame("127.0.0.1:5100");

        int oldBalance = gameManager.GetBalance();

        // Wait for the balance to change
        Thread.Sleep(1003);

        //Assert
        Assert.True(oldBalance < gameManager.GetBalance());
    }
}
