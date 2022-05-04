using DesktopHostingClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace DesktopHostingClient.Service;

public class PurchasableService
{
    public async Task<List<Purchasable>> GetPurchasables()
    {
        HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.GetAsync("https://localhost:7236/Purchasable");

        List<Purchasable> foundPurchasable = null;

        if (response.IsSuccessStatusCode)
        {
            foundPurchasable = await response.Content.ReadFromJsonAsync<List<Purchasable>>();
        }

        return foundPurchasable;
    }
}
