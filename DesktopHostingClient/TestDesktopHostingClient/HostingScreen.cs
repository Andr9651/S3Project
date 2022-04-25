using DesktopHostingClient.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestDesktopHostingClient;

public class HostingScreen
{
    [Fact]
    public async void TestGetHostIp ()
    {
        //Arrange
        HostingManager hostingManager = new HostingManager();
        //Act 
        string hostIp = await hostingManager.GetPublicIP();
        //Assert
        Assert.NotNull(hostIp);
    }
}
