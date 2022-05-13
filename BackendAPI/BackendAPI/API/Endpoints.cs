using ModelLibrary.Model;
using Dapper;
using BackendAPI.DBModel;
using BackendAPI.Service;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.API;

public class Endpoints
{

    public static void SetupEndpoints(WebApplication webApplication)
    {
        webApplication.MapGet("/Purchasable", () =>
        {
            string connectionString = webApplication.Configuration.GetConnectionString("ConnectMsSqlString");

            SQLGameDataService gameDataService = new SQLGameDataService(connectionString);

            List<DBPurchasable> purchasables = gameDataService.GetPurchasables();

            IResult HTTPResult;

            if (purchasables is null)
            {
                HTTPResult = Results.StatusCode(500);
            }
            else
            {
                HTTPResult = Results.Ok(purchasables);
            }

            return HTTPResult;
        });

        webApplication.MapGet("/GameData/{id}", (int id) =>
        {
            string connectionString = webApplication.Configuration.GetConnectionString("ConnectMsSqlString");

            SQLGameDataService gameDataService = new SQLGameDataService(connectionString);

            GameData gameData = gameDataService.GetGameData(id);

            IResult HTTPResult;

            if (gameData is null)
            {
                HTTPResult = Results.NotFound();
            }
            else
            {
                HTTPResult = Results.Ok(gameData);
            }

            return HTTPResult;
        });

        webApplication.MapPost("/GameData", () =>
        {
            string connectionString = webApplication.Configuration.GetConnectionString("ConnectMsSqlString");

            SQLGameDataService gameDataService = new SQLGameDataService(connectionString);

            GameData gameData = gameDataService.CreateGameData();

            IResult HTTPResult;

            if (gameData is null || gameData.Id == 0)
            {
                HTTPResult = Results.StatusCode(500);
            }
            else
            {
                HTTPResult = Results.Ok(gameData);
            }
            return HTTPResult;
        });

        webApplication.MapPut("/GameData", ([FromBody] GameData gameData) =>
        {
            string connectionString = webApplication.Configuration.GetConnectionString("ConnectMsSqlString");

            SQLGameDataService gameDataService = new SQLGameDataService(connectionString);

            bool Success = gameDataService.SaveGameData(gameData);

            IResult HTTPResult;

            if (Success)
            {
                HTTPResult = Results.Ok();
            }
            else
            {
                HTTPResult = Results.BadRequest();
            }

            return HTTPResult;
        });
    }
}
