using BackendAPI.Model;
using Dapper;
using System.Data.SqlClient;

namespace BackendAPI.API;

public class Endpoints
{

    public static void SetupEndpoints(WebApplication webApplication)
    {
        webApplication.MapGet("/Persons", () =>
        {
            string connectionString = "data Source=hildur.ucn.dk; Database=; User Id=dmaa0221_1089445;Password=Password1!;";

            List<Person>? persons = null;

            string sqlQuery = "select * from persons";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                persons = connection.Query<Person>(sqlQuery).ToList();
            }

            return persons;
        });

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
