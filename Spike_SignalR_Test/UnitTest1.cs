using Xunit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using Spike_SignalR;
using Moq;
using System.Dynamic;
using System;

namespace Spike_SignalR_Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            System.Console.WriteLine("ha");

            Assert.Equal(1, 0);
        }


        [Theory]
        [InlineData("Candy Shop", "The Candy Shop was bought")]
        public async void Test2(string building, string expected)
        {
            ShopHub hub = new ShopHub();

            var mockClients = new Mock<IHubCallerClients>();

            var mockContext = new Mock<HubCallerContext>();

            var mockGroup = new Mock<IGroupManager>();

            hub.Clients = mockClients.Object;
            hub.Context = mockContext.Object;
            hub.Groups = mockGroup.Object;

            string buyResponse = await hub.BuyBuilding(building);

            Assert.Equal(expected, buyResponse);
        }


        [Fact]
        public async void HubsAreMockableViaDynamic()
        {
            bool sendCalled = false;
            var hub = new ShopHub();
            var mockClients = new Mock<IHubCallerClients>();
            dynamic f = new ExpandoObject();
            hub.Clients = (IHubCallerClients)f;
            dynamic g = new ExpandoObject();
            hub.Clients.All = (IClientProxy)g;
            dynamic all = new ExpandoObject();
            all.SendAsync = new Action<string, string>((string1, string2) => {
                sendCalled = true;
            });
            mockClients.Setup(m => m.All).Returns(all);
            //hub.Send("TestUser", "TestMessage");
            string buyResponse = await hub.BuyBuilding("Candy shop");


            Assert.True(sendCalled);
        }

    }


}