using GenericMongoRepository.Extensions;
using GenericMongoRepository.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var s = builder.Configuration.GetConnectionString("MongoDb");
var mongoClient = new MongoClient(s);
var a = mongoClient.GetDatabase("WW3");
builder.Services.AddMongoRepositories(a);
builder.Services.AddSingleton<BattlePassService>();

var app = builder.Build();

app.MapGet("/get/{id:long}", (BattlePassService service, long id) => service.GetByIdAsync(id));
app.MapGet("/getList", (BattlePassService service, [FromQuery] int take, [FromQuery] int skip) => service.GetAllAsync(skip, take));

app.Run();