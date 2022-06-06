using ModelLibrary.Model;
using BackendAPI.DBModel;
using BackendAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.API;

public static class Endpoints
{
    private static string _connectionString;

    public static void SetupEndpoints(this WebApplication webApplication)
    {
        _connectionString = webApplication.Configuration.GetConnectionString("ConnectMsSqlString");

        webApplication.MapGet("/Purchasable", GetPurchasable);
        webApplication.MapGet("/GameData/{id}", GetGameData);
        webApplication.MapPost("/GameData", PostGameData);
        webApplication.MapPut("/GameData", PutGameData);
    }

    // Return JSON object with all available Purchaseables with the
    // purchasable's Id as keys.
    private static IResult GetPurchasable(SQLGameDataService gameDataService)
    {
        Dictionary<int, DBPurchasable> purchasables = gameDataService.GetPurchasables();
        bool success = purchasables is not null;

        return success ? Results.Ok(purchasables) : Results.StatusCode(500);
    }

    private static IResult GetGameData(int id, SQLGameDataService gameDataService)
    {
        GameData gameData = gameDataService.GetGameData(id);
        bool success = gameData is not null;

        return success ? Results.Ok(gameData) : Results.NotFound();
    }

    private static IResult PostGameData(SQLGameDataService gameDataService)
    {
        GameData gameData = gameDataService.CreateGameData();
        bool success = gameData is not null && gameData.Id > 0;

        return success ? Results.Ok(gameData) : Results.StatusCode(500);
    }

    // The [FromBody] attribute, specifies that the gamedata parameter should
    // get its value from the HTTP Request Body.
    private static IResult PutGameData([FromBody] GameData gameData, SQLGameDataService gameDataService)
    {

        bool Success = gameDataService.SaveGameData(gameData);

        return Success ? Results.Ok() : Results.BadRequest();
    }
}
