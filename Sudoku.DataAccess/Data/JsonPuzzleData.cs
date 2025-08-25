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
        return await _httpClient.GetFromJsonAsync<List<PuzzleModel>>("/blazor/data/puzzles.json") ?? [];
    }
}