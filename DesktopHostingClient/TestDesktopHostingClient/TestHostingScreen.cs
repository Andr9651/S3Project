using DesktopHostingClient.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestDesktopHostingClient;

[Collection("Sequential")]
public class TestHostingScreen
{
    [Fact]
    public async void TestGetHostIp ()
    {
        //Arrange
        HostingManager hostingManager = new HostingManager();
        //Act 
        string hostIp = await hostingManager.GetPublicIp();
        //Assert
        Assert.NotNull(hostIp);
    }
}
