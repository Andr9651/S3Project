using Xunit;
using System.Data.SqlClient;
using Dapper;
using ModelLibrary.Model;
using BackendAPI.DBModel;
using BackendAPI.Service;
using Microsoft.Extensions.Configuration;

namespace TestBackendAPI;

public class TestDapper
{
    private string _dbConnectionString;

    public TestDapper()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        _dbConnectionString = config.GetConnectionString("ConnectMsSqlString");
    }

    [Fact]
    [Trait("UserStory", "SQL Database")]
    public void TestConnection()
    {
        //arrange
        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            //act
            string version = connection.ExecuteScalar<string>("select @@version");

            //assert
            Assert.NotNull(version);
            System.Console.WriteLine(version);
        }
    }

    [Fact]
    [Trait("UserStory", "SQL Database")]
    public void TestDBPurchasable()
    {
        //arrange
        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            //act
            //start with test insert
            DBPurchasable purchasable = new DBPurchasable
            {
                Name = "TestItem",
                Price = 69420
            };

            string sqlInsert = "insert into Purchasable(name, price) values (@Name, @Price)";
            int rowsInserted = connection.Execute(sqlInsert, purchasable);

            //assert
            Assert.Equal(1, rowsInserted);

            if (rowsInserted >= 1)
            {
                //testDelete
                string sqlDelete = "delete from Purchasable where name = @Name";
                int rowsDeleted = connection.Execute(sqlDelete, purchasable);
                Assert.True(rowsDeleted > 0);
            }

        }
    }
    [Fact]
    [Trait("UserStory", "SQL Database")]
    public void TestDBGameData()
    {
        //arrange
        using (SqlConnection connection = new SqlConnection(_dbConnectionString))
        {
            //act
            //start with test insert
            DBGameData dbGameData = new DBGameData
            {
                Balance = 1,
                HostIp = "Jeg er ikke en ip"
            };

            string sqlInsert = "insert into GameData(balance, hostIp) values (@Balance, @HostIp)";
            int rowsInserted = connection.Execute(sqlInsert, dbGameData);

            //assert
            Assert.Equal(1, rowsInserted);
            if (rowsInserted >= 1)
            {
                //testDelete
                string sqlDelete = "delete from GameData where hostIp = @HostIp";
                int rowsDeleted = connection.Execute(sqlDelete, dbGameData);
                Assert.True(rowsDeleted > 0);
            }
        }
    }

    [Fact]
    [Trait("UserStory", "Create New Game Instance")]
    public void TestCreateGameData()
    {
        //Arrange 
        SQLGameDataService sqlGameDataService = new SQLGameDataService(_dbConnectionString);

        //Act 
        GameData gameData = sqlGameDataService.CreateGameData();

        //Assert
        Assert.NotNull(gameData);
        Assert.NotEqual(0, gameData.Id);
    }

    [Fact]
    [Trait("UserStory", "Save Game")]
    public void TestSaveGameData()
    {
        //Arrange 
        SQLGameDataService sqlGameDataService = new SQLGameDataService(_dbConnectionString);

        //Act 
        GameData gameData = sqlGameDataService.CreateGameData();
        gameData.Ip = "Jeg er ikke en ip";

        bool result = sqlGameDataService.SaveGameData(gameData);

        //Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("UserStory", "Save Game")]
    public void TestSaveGameDataWithPurchases()
    {
        //Arrange 
        SQLGameDataService sqlGameDataService = new SQLGameDataService(_dbConnectionString);

        //Act 
        GameData gameData = sqlGameDataService.CreateGameData();
        gameData.Purchases.Add(1, 5);
        gameData.Purchases.Add(2, 5);

        bool result = sqlGameDataService.SaveGameData(gameData);

        //Assert
        Assert.True(result);
    }

    [Fact]
    [Trait("UserStory", "Load Game Instance")]
    public void TestLoadGameData()
    {
        //Arrange
        SQLGameDataService sqlGameDataService = new SQLGameDataService(_dbConnectionString);

        GameData gameData = sqlGameDataService.CreateGameData();
        gameData.Purchases.Add(1, 5);
        gameData.Purchases.Add(2, 5);

        sqlGameDataService.SaveGameData(gameData);

        //Act
        GameData loadedGameData = sqlGameDataService.GetGameData(gameData.Id);

        //Assert
        Assert.NotNull(loadedGameData);
        Assert.Equal(2, loadedGameData.Purchases.Count);
        Assert.Equal(5, loadedGameData.Purchases[1]);
        Assert.Equal(5, loadedGameData.Purchases[2]);
    }

    [Fact]
    [Trait("UserStory", "Load Game Instance")]
    public void TestLoadGameDataWithoutPurchases()
    {
        //Arrange
        SQLGameDataService sqlGameDataService = new SQLGameDataService(_dbConnectionString);

        GameData gameData = sqlGameDataService.CreateGameData();

        sqlGameDataService.SaveGameData(gameData);

        //Act
        GameData loadedGameData = sqlGameDataService.GetGameData(gameData.Id);

        //Assert
        Assert.NotNull(loadedGameData);
        Assert.Equal(0, loadedGameData.Purchases.Count);
    }

    [Fact]
    [Trait("UserStory", "Load Game Instance")]
    public void TestLoadGameDataBadId()
    {
        SQLGameDataService sqlGameDataService = new SQLGameDataService(_dbConnectionString);

        GameData gameData = sqlGameDataService.GetGameData(-1);

        //Assert 
        Assert.Null(gameData);
    }
}
