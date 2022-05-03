using Xunit;
using System.Data.SqlClient;
using Dapper;
using BackendAPI.Model;

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

    }
}