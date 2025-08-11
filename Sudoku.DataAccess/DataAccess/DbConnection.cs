using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Sudoku.DataAccess.Models;

namespace Sudoku.DataAccess.DataAccess;

public class DbConnection : IDbConnection
{
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _db;
    private string _connectionId = "MongoDB";
    public string PuzzleCollectionName { get; private set; } = "puzzles";

    public MongoClient Client { get; private set; }
    public IMongoCollection<PuzzleModel> PuzzleCollection { get; private set; }
    
    public DbConnection(IConfiguration config) {
        _config = config;
        Client = new MongoClient(_config.GetConnectionString(_connectionId));
        _db = Client.GetDatabase(_config["DatabaseName"]);

        PuzzleCollection = _db.GetCollection<PuzzleModel>(PuzzleCollectionName);
    }
}