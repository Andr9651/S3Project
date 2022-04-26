using Xunit;
using WebApp.Managers;
using System;

namespace TestWebApp;
public class TestJoinGame
{
    [Theory]
    [InlineData("192.168.1.15:8080", true)]
    [InlineData("600.900.-1.000:8", false)]
    [InlineData("600.900.120.000:8080", false)]
    [InlineData("192.168.1.15", false)]
    public void TestValidateIP(string ip, bool expected)
    {
        //Arrange

        //Act
        bool result = JoinGameManager.ValidateIP(ip);
        //Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("192.168.1.15:8080", "/Game/192.168.1.15:8080")]
    [InlineData("Foobar", "/Game/Foobar")]
    public void TestCreateJoinGameURI(string ip, string expected)
    {
        //Arrange
        //Act
        string result = JoinGameManager.CreateJoinGameURI(ip);
        //Assert
        Assert.Equal(expected, result);
    }
    [Fact]
    public void TestServerPing()
    {
        //Arrange
        GameManager gameManager = new GameManager();
        //Act
        gameManager.ConnectToGame("127.0.0.1:5100");

        bool receivedPong = false;
        Action pongAction = () => { receivedPong = true; };
        gameManager.PongEvent += pongAction;

        gameManager.PingServer();

        //Assert

        Assert.True(receivedPong);
    }

}