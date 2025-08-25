using MongoDB.Driver;
using Sudoku.DataAccess.DataAccess;
using Sudoku.DataAccess.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Sudoku.DataAccess.Data;

public class MongoPuzzleData : IPuzzleData
{
    private readonly IMongoCollection<PuzzleModel> _puzzles;
    private readonly IMemoryCache _cache;
    private const string CacheName = "PuzzleData";
    
    public MongoPuzzleData(IDbConnection db, IMemoryCache cache) {
        _cache = cache;
        _puzzles = db.PuzzleCollection;
    }

    public async Task<List<PuzzleModel>> GetAllPuzzles() {
        Console.WriteLine("Mongo");
        var output = _cache.Get<List<PuzzleModel>>(CacheName);
        if (output is null) {
            var results = await _puzzles.FindAsync(_ => true);
            output = results.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromDays(1));
        }
        
        return output;
    }

    public async Task<PuzzleModel> GetPuzzle(string id) {
        var results = await _puzzles.FindAsync(p => p.Id == id);
        return results.FirstOrDefault();
    }

    public Task CreatePuzzle(PuzzleModel puzzle) {
        return _puzzles.InsertOneAsync(puzzle);
    }
}