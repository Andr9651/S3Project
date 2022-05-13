using ModelLibrary.Model;
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

    public PurchasableService(string apiUrl)
    {
        _apiUrl = apiUrl;
    }

    public async Task<Dictionary<int, Purchasable>> GetPurchasables()
    {
        HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.GetAsync($"{_apiUrl}/Purchasable");

        Dictionary<int, Purchasable> foundPurchasables = null;

        if (response.IsSuccessStatusCode)
        {
            foundPurchasables = await response.Content.ReadFromJsonAsync<Dictionary<int, Purchasable>>();
        }

        return foundPurchasables;
    }
}
