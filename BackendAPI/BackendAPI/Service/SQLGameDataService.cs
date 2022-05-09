using BackendAPI.Model.DTO;
using System.Data.SqlClient;
using Dapper;

namespace BackendAPI.Service;

public class SQLGameDataService
{
    private string _dbConnectionString = "Data Source=.;Initial Catalog=CookieClicker;Integrated Security=True";

    public List<PurchasableDto> GetPurchasables()
    {
        List<PurchasableDto>? purchasableDtos = null;

        string sqlQuery = "select * from Purchasable";

        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            purchasableDtos = connection.Query<PurchasableDto>(sqlQuery).ToList();
        }

        return purchasableDtos;
    }

    public bool SaveGameInstance(GameInstance gameInstance)
    {
        bool result = false;

        string sqlQueryUpdateGameInstance = "";
        string sqlQueryUpdateGamePurchases = "";

        GameInstanceDto gameInstanceDto = gameInstance.GetGameInstanceDto();

        List<GamePurchaseDto> gamePurchaseDtoList = gameInstance.GetGamePurchaseDtos();

        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                bool error = false;

                int linesChangedGameInstance = connection.Execute(
                    sqlQueryUpdateGameInstance,
                    gameInstanceDto
                );

                if (linesChangedGameInstance == 0)
                {
                    error = true;
                }

                if (!error && gamePurchaseDtoList.Count > 0)
                {
                    int linesChangedGamePurchases = connection.Execute(
                        sqlQueryUpdateGamePurchases,
                        gamePurchaseDtoList
                    );

                    if (linesChangedGamePurchases == 0)
                    {
                        error = true;
                    }
                }

                if (error)
                {
                    transaction.Rollback();
                    result = false;
                }
                else
                {
                    transaction.Commit();
                    result = true;
                }
            }
        }

        return result;
    }
}

