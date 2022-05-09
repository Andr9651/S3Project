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

        string sqlQueryUpdateGameInstance = "Update GameInstance " +
            "set balance = @balance, hostIp = @hostIp " +
            "where id = @id";
        string sqlQueryUpdateGamePurchases = "insert into GamePurchase " +
            "Values (@gamenstanceId, @purchasableId, @amount)";

        GameInstanceDto gameInstanceDto = gameInstance.GetGameInstanceDto();

        List<GamePurchaseDto> gamePurchaseDtoList = gameInstance.GetGamePurchaseDtos();

        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            connection.Open();
            using (SqlTransaction SqlTransaction = connection.BeginTransaction())
            {
                bool error = false;

                int linesChangedGameInstance = connection.Execute(
                    sqlQueryUpdateGameInstance,
                    gameInstanceDto,
                    SqlTransaction
                );

                if (linesChangedGameInstance == 0)
                {
                    error = true;
                }

                if (!error && gamePurchaseDtoList.Count > 0)
                {
                    int linesChangedGamePurchases = connection.Execute(
                        sqlQueryUpdateGamePurchases,
                        gamePurchaseDtoList,
                        SqlTransaction

                    );

                    if (linesChangedGamePurchases == 0)
                    {
                        error = true;
                    }
                }

                if (error)
                {
                    SqlTransaction.Rollback();
                    result = false;
                }
                else
                {
                    SqlTransaction.Commit();
                    result = true;
                }
            }
        }

        return result;
    }

    public GameInstance CreateGameInstance()
    {
        GameInstanceDto gameInstanceDto = new GameInstanceDto();

        string sqlQueryCreateGameInstance = "insert into GameInstance " +
            "output inserted.id " +
            "values (@Balance, @Hostip)";

        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            gameInstanceDto.Id = connection.QuerySingle<int>(sqlQueryCreateGameInstance, gameInstanceDto);
        }
        GameInstance gameInstance = new GameInstance(gameInstanceDto);

        return gameInstance;
    }
}

