using BackendAPI.Model;
using Dapper;
using BackendAPI.Model.DTO;
using BackendAPI.Service;
using System.Data.SqlClient;

namespace BackendAPI.API;

public class Endpoints
{

    public static void SetupEndpoints(WebApplication webApplication)
    {
        webApplication.MapGet("/Purchasable", () =>
        {
            SQLGameDataService gameDataService = new SQLGameDataService();

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




    }
}
