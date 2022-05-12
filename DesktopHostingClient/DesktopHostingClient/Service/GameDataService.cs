using DesktopHostingClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DesktopHostingClient.Service;

public class GameDataService
{
    private string _apiUrl;

    public GameDataService()
    {
        _apiUrl = ConfigurationManager.ConnectionStrings["APIConnectionString_LocalHost"].ToString();
    }

    public GameDataService(string apiUrl)
    {
        _apiUrl = apiUrl;
    }

    public async Task<GameData> CreateGameData()
    {
        HttpClient client = new HttpClient();

        HttpContent content = new StringContent("");

        HttpResponseMessage response = await client.PostAsync($"{_apiUrl}/GameInstance", content);

        GameData gameData = null;

        if (response.IsSuccessStatusCode)
        {
            gameData = await response.Content.ReadFromJsonAsync<GameData>();
        }
        return gameData;
    }

    public async Task<bool> SaveGameData(GameData gameData)
    {
        HttpClient client = new HttpClient();

        HttpContent content = JsonContent.Create(gameData);

        HttpResponseMessage response = await client.PutAsync($"{_apiUrl}/GameInstance", content);

        return response.IsSuccessStatusCode;
    }

    public async Task<GameData> LoadGameData(int id)
    {
        HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.GetAsync($"https://localhost:7236/GameInstance/{id}");

        GameData gameData = null;

        if (response.IsSuccessStatusCode)
        {
            gameData = await response.Content.ReadFromJsonAsync<GameData>();
        }
        return gameData;
    }
}
