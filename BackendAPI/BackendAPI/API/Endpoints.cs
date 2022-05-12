using BackendAPI.Model;
using Dapper;
using BackendAPI.Model.DTO;
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
            string connectionString = webApplication.Configuration.GetConnectionString("SQLConnectionString");

            SQLGameDataService gameDataService = new SQLGameDataService(connectionString);

            List<PurchasableDto> purchasables = gameDataService.GetPurchasables();

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

        webApplication.MapGet("/GameInstance/{id}", (int id) =>
        {
            string connectionString = webApplication.Configuration.GetConnectionString("SQLConnectionString");

            SQLGameDataService gameDataService = new SQLGameDataService(connectionString);

            GameInstance gameInstance = gameDataService.GetGameInstance(id);

            IResult HTTPResult;

            if (gameInstance is null)
            {
                HTTPResult = Results.NotFound();
            }
            else
            {
                HTTPResult = Results.Ok(gameInstance);
            }

            return HTTPResult;
        });

        webApplication.MapPost("/GameInstance", () =>
        {
            string connectionString = webApplication.Configuration.GetConnectionString("SQLConnectionString");

            SQLGameDataService gameDataService = new SQLGameDataService(connectionString);

            GameInstance gameInstance = gameDataService.CreateGameInstance();

            IResult HTTPResult;

            if (gameInstance is null || gameInstance.Id == 0)
            {
                HTTPResult = Results.StatusCode(500);
            }
            else
            {
                HTTPResult = Results.Ok(gameInstance);
            }
            return HTTPResult;
        });

        webApplication.MapPut("/GameInstance", ([FromBody] GameInstance gameInstance) =>
        {
            string connectionString = webApplication.Configuration.GetConnectionString("SQLConnectionString");

            SQLGameDataService gameDataService = new SQLGameDataService(connectionString);

            bool Success = gameDataService.SaveGameInstance(gameInstance);

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
