using Sudoku.Core.Models;

namespace Sudoku.DataAccess.Services;

public class SudokuSolver
{
    public int GridRows { get; set; } = 9;
    public int GridCols { get; set; } = 9;
    public int BoxRows { get; set; } = 3;
    public int BoxCols { get; set; } = 3;
    
    public bool IsSolved(Grid grid) {
        HashSet<string> seen = [];

        for (int row = 0; row < GridRows; row++) {
            for (int col = 0; col < GridCols; col++) {
                char? value = grid.GetCell(row, col).Value;

                if (value is < '1' or > '9') return false;
                
                string rowKey = $"{value} in row {row}";
                string colKey = $"{value} in col {col}";
                string boxKey = $"{value} in box {row / BoxRows}:{col / BoxCols}";

                if (!seen.Add(rowKey) || !seen.Add(colKey) || !seen.Add(boxKey)) return false;
            }
        }

        return true;
    }
}