using BackendAPI.API;
using BackendAPI.Model;
using Dapper;
using System.Data.SqlClient;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//app.UseAuthorization();

Endpoints.SetupEndpoints(app);

app.Run();
