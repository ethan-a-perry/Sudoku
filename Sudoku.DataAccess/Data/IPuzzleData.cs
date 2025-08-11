using Sudoku.DataAccess.Models;

namespace Sudoku.DataAccess.Data;

public interface IPuzzleData
{
    Task<List<PuzzleModel>> GetAllPuzzles();
    Task<PuzzleModel> GetPuzzle(string id);
    
    Task CreatePuzzle(PuzzleModel puzzle);
}