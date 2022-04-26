using Xunit;
using WebApp.Managers;

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
}
