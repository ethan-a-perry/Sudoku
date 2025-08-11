using MongoDB.Driver;
using Sudoku.DataAccess.Models;

namespace Sudoku.DataAccess.DataAccess;

public interface IDbConnection
{
    string PuzzleCollectionName { get; }
    MongoClient Client { get; }
    IMongoCollection<PuzzleModel> PuzzleCollection { get; }
}