using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spike_SignalR.BackgroundServices
{
    public class BalanceUpdater : BackgroundService
    {
        private IHubContext<ShopHub> shopHub;
        static public int balance = 0;

        public BalanceUpdater(IHubContext<ShopHub> shopHub)
        {
            this.shopHub = shopHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                balance++;
                await shopHub.Clients.All.SendAsync("BalanceUpdate", balance );
                await Task.Delay(1, stoppingToken);
            }
        }
    }
}
