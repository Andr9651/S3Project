using Xunit;
using System.Data.SqlClient;
using Dapper;
using BackendAPI.Model;
using BackendAPI.Model.DTO;
using BackendAPI.Service;

namespace TestBackendAPI
{
    public class TestDapper
    {
        [Fact]
        public void TestLocalConnection()
        {
            //arrange
            string connectionString = "Data Source=.;Initial Catalog=CookieClicker;Integrated Security=True";
            using SqlConnection connection = new SqlConnection(connectionString);
            //act
            connection.Open();
            string version = connection.ExecuteScalar<string>("select @@version");
            //assert
            Assert.NotNull(version);
            System.Console.WriteLine(version);
        }

        [Fact]
        public void TestPurchasableDto()
        {
            //arrange
            string connectionString = "Data Source=.;Initial Catalog=CookieClicker;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //act
                //start with test insert
                PurchasableDto purchasable = new PurchasableDto
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
        public void TestGameInstanceDto()
        {
            //arrange
            string connectionString = "Data Source=.;Initial Catalog=CookieClicker;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //act
                //start with test insert
                GameInstanceDto gameInstanceDto = new GameInstanceDto
                {
                    Balance = 1,
                    HostIp = "Jeg er ikke en ip"
                };
                string sqlInsert = "insert into GameInstance(balance, hostIp) values (@Balance, @HostIp)";
                int rowsInserted = connection.Execute(sqlInsert, gameInstanceDto);
                //assert
                Assert.Equal(1, rowsInserted);
                if (rowsInserted >= 1)
                {
                    //testDelete
                    string sqlDelete = "delete from GameInstance where hostIp = @HostIp";
                    int rowsDeleted = connection.Execute(sqlDelete, gameInstanceDto);
                    Assert.True(rowsDeleted > 0);
                }
            }
        }

        [Fact]
        public void TestCreateGameInstance()
        {
            //Arrange 
            SQLGameDataService sqlGameDataService = new SQLGameDataService();
            //Act 
            GameInstance gameInstance = sqlGameDataService.CreateGameInstance();
            //Assert
            Assert.NotNull(gameInstance);
            Assert.NotEqual(0, gameInstance.Id);

        }
        [Fact]
        public void TestSaveGameInstance()
        {
            //Arrange 
            SQLGameDataService sqlGameDataService = new SQLGameDataService();

            //Act 
            GameInstance gameInstance = sqlGameDataService.CreateGameInstance();
            gameInstance.HostIp = "Jeg er ikke en ip";

            bool result = sqlGameDataService.SaveGameInstance(gameInstance);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void TestSaveGameInstanceWithPurchases()
        {
            //Arrange 
            SQLGameDataService sqlGameDataService = new SQLGameDataService();
            //Act 
            GameInstance gameInstance = sqlGameDataService.CreateGameInstance();
            gameInstance.Purchases.Add(2, 5);
            gameInstance.Purchases.Add(3, 5);

            bool result = sqlGameDataService.SaveGameInstance(gameInstance);
            //Assert

            Assert.True(result);
        }
    }
}