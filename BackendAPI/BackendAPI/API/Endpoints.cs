using BackendAPI.Model;
using Dapper;
using System.Data.SqlClient;

namespace BackendAPI.API;

public class Endpoints
{

    public static void SetupEndpoints(WebApplication webApplication)
    {
        webApplication.MapGet("/Purchasable", () =>
        {
            string connectionString = "Data Source=.;Initial Catalog=CookieClicker;Integrated Security=True";

            List<PurchasableDto>? purchasableDtos = null;

            string sqlQuery = "select * from Purchasable";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                purchasableDtos = connection.Query<PurchasableDto>(sqlQuery).ToList();
            }

            return purchasableDtos;
        });


    }
}
