using ModelLibrary.Model;
using BackendAPI.DBModel;
using BackendAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.API;

public class Endpoints
{
    public static void SetupEndpoints(WebApplication webApplication)
    {
        // Return JSON object with all available Purchaseables with the
        // purchasable's Id as keys.
        webApplication.MapGet("/Purchasable", () =>
        {
            // First get the purchasables from the database.
            string connectionString = webApplication.Configuration.GetConnectionString("ConnectMsSqlString");
            SQLGameDataService gameDataService = new SQLGameDataService(connectionString);
            Dictionary<int, DBPurchasable> purchasables = gameDataService.GetPurchasables();

            // Construct the HTTP result with proper status code.
            // If there is no need for status codes AspNetCore will convert
            // a returned value to HTTPResult with status code 200 on it's own.
            IResult HTTPResult;
            if (purchasables is null)
            {
                // 500 = server error
                HTTPResult = Results.StatusCode(500);
            }
            else
            {
                // Status code for OK = 200
                // This method automatically converts the dictionary to a JSON object.
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

    // This is how it would have looked as an extension method 
    // calling it would look like this:
    // app.SetupEndpoints();
    // public static void SetupEndpoints(this WebApplication webApplication)
    // {
    //     ... 
    // }
}
