using Sudoku.Core.Models;

namespace Sudoku.DataAccess.Services;

public class SudokuSolver
{
    public int GridRows { get; set; } = 9;
    public int GridCols { get; set; } = 9;
    public int BoxRows { get; set; } = 3;
    public int BoxCols { get; set; } = 3;
    
    public bool IsSolved(Cell[,] cells) {
        HashSet<string> seen = [];

        for (int r = 0; r < GridRows; r++) {
            for (int c = 0; c < GridCols; c++) {
                char? value = cells[r, c].Value;

                if (value == 0 || value is null) return false;

                string rowKey = $"{value} in row {r}";
                string colKey = $"{value} in col {c}";
                string boxKey = $"{value} in box {r / BoxRows}:{c / BoxCols}";

                if (!seen.Add(rowKey) || !seen.Add(colKey) || !seen.Add(boxKey)) return false;
            }
        }

        return true;
    }
}