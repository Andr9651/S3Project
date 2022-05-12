using DesktopHostingClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Configuration;

namespace DesktopHostingClient.Service;
public class PurchasableService
{
    private string _apiUrl;
    public PurchasableService()
    {
        _apiUrl = ConfigurationManager.ConnectionStrings["APIConnectionString_LocalHost"].ToString();
    }
    public async Task<List<Purchasable>> GetPurchasables()
    {
        HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.GetAsync($"{_apiUrl}/Purchasable");

        List<Purchasable> foundPurchasable = null;

        if (response.IsSuccessStatusCode)
        {
            foundPurchasable = await response.Content.ReadFromJsonAsync<List<Purchasable>>();
        }

        return foundPurchasable;
    }
}
