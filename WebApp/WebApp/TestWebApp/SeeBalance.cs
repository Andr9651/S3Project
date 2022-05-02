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
    public void TestSeebalance()
    {
        //Arrange 
        GameManager gameManager = new GameManager();

        //Act 
        gameManager.ConnectToGame("127.0.0.1:5100");
        bool receivedUpdate = false;
        gameManager.BalanceUpdateEvent += (balance) =>
        {
            receivedUpdate = true;
        };
        Thread.Sleep(500);
       
        //Assert
        Assert.True(receivedUpdate);
    }
}
