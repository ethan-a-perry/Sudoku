using Sudoku.DataAccess.Models;

namespace Sudoku.DataAccess.Data;

public interface IPuzzleData
{
    Task<List<PuzzleModel>> GetAllPuzzles();
}