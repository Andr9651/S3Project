﻿using BackendAPI.DBModel;
using ModelLibrary.Model;
using System.Data.SqlClient;
using Dapper;
using System.Configuration;
using System.Linq;

namespace BackendAPI.Service;

public class SQLGameDataService
{
    private readonly string _dbConnectionString;

    public SQLGameDataService(string connectionString)
    {
        _dbConnectionString = connectionString;
    }

    public SQLGameDataService(IConfiguration configuration)
    {
        _dbConnectionString = configuration.GetConnectionString("ConnectMsSqlString");
    }


    /// <returns> A Dictionary mapping a Purchaseable.Id to Purchaseable</returns>
    public Dictionary<int, DBPurchasable> GetPurchasables()
    {
        Dictionary<int, DBPurchasable> dbPurchasables = null;

        string sqlQuery = "select * from Purchasable";

        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            dbPurchasables = connection.Query<DBPurchasable>(sqlQuery).ToDictionary(p => p.Id);
        }

        return dbPurchasables;
    }

    public bool SaveGameData(GameData gameData)
    {
        bool result = false;

        string sqlQueryUpdateGameData = "Update GameData " +
            "set balance = @balance " +
            "where id = @id";
        string sqlQueryUpdateGamePurchases = "update gamepurchase " +
            "set amount = @amount " +
            "where gameDataId = @gameDataId and purchasableId = @purchasableId " +
            "if @@rowcount = 0 " +
            "insert into GamePurchase " +
            "Values (@gameDataId, @purchasableId, @amount)";

        DBGameData dbGameData = ModelConverter.ToDBGameData(gameData);

        List<DBGamePurchase> dbGamePurchases = ModelConverter.ToDBGamePurchases(gameData);

        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            // connection.Open() is needed to start a transaction.
            connection.Open();
            using (SqlTransaction SqlTransaction = connection.BeginTransaction())
            {
                bool error = false;

                int linesChangedGameData = connection.Execute(
                    sqlQueryUpdateGameData,
                    dbGameData, // acts as parameters.
                    SqlTransaction
                );

                if (linesChangedGameData == 0)
                {
                    error = true;
                }

                if (!error && dbGamePurchases.Count > 0)
                {
                    int linesChangedGamePurchases = connection.Execute(
                        sqlQueryUpdateGamePurchases,
                        dbGamePurchases, // acts as parameters.
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

    public GameData CreateGameData()
    {
        DBGameData dbGameData = new DBGameData();

        string sqlQueryCreateGameData = "insert into GameData " +
            "output inserted.id " +
            "values (@Balance)";

        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            dbGameData.Id = connection.QuerySingle<int>(sqlQueryCreateGameData, dbGameData);
        }
        GameData gameData = ModelConverter.ToGameData(dbGameData);

        return gameData;
    }

    public GameData GetGameData(int id)
    {
        GameData gameData = null;

        string sqlQueryGetDBGameData = "select * from GameData " +
            "where id = @id ";
        string sqlQueryGetDBGamePurchases = "select * from GamePurchase " +
            "where GameDataId = @id";

        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            DBGameData dbGameData = null;
            // The id in the anonymous object needs to match the @id in the query
            var parameters = new { id = id };
            dbGameData = connection.QuerySingleOrDefault<DBGameData>(
                sqlQueryGetDBGameData,
                parameters
            );

            if (dbGameData is not null)
            {
                List<DBGamePurchase> dbGamePurchases = null;
                dbGamePurchases = connection.Query<DBGamePurchase>(
                    sqlQueryGetDBGamePurchases,
                    parameters
                ).ToList();

                gameData = ModelConverter.ToGameData(dbGameData, dbGamePurchases);
            }
        }

        return gameData;
    }
}

