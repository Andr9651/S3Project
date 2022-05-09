using BackendAPI.Model.DTO;
using System.Data.SqlClient;
using Dapper;

namespace BackendAPI.Service;

public class SQLGameDataService
{

    public List<PurchasableDto> GetPurchasables()
    {
        string connectionString = "Data Source=.;Initial Catalog=CookieClicker;Integrated Security=True";

        List<PurchasableDto>? purchasableDtos = null;

        string sqlQuery = "select * from Purchasable";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            purchasableDtos = connection.Query<PurchasableDto>(sqlQuery).ToList();
        }

        return purchasableDtos;
    }

}

