using System.Net.Http.Json;
using Sudoku.DataAccess.Models;

namespace Sudoku.DataAccess.Data;

public class JsonPuzzleData : IPuzzleData
{
    private readonly HttpClient _httpClient;

    public JsonPuzzleData(HttpClient httpClient) {
        _httpClient = httpClient;
    }
    public async Task<List<PuzzleModel>> GetAllPuzzles() { 
        Console.WriteLine("Json");
        return await _httpClient.GetFromJsonAsync<List<PuzzleModel>>("/blazor/data/puzzles.json") ?? [];
    }

    public async Task<PuzzleModel> GetPuzzle(string id) {
        var puzzles = await GetAllPuzzles();
        return puzzles.FirstOrDefault(p => p.Id == id);
    }

    public Task CreatePuzzle(PuzzleModel puzzle) {
        throw new NotSupportedException("Cannot create puzzles in WASM JSON service.");
    }
}