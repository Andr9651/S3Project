using BackendAPI.Model;
using Dapper;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);
// Little comment.
// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();



app.MapGet("/Persons", () =>
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

app.Run();
