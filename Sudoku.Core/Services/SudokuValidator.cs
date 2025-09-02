namespace Sudoku.DataAccess.Services;

public class SudokuValidator
{
    public bool IsValid(List<char> grid, int numGridRows, int numGridCols, int numBoxRows, int numBoxCols) {
        if (numGridRows * numGridCols != grid.Count) return false;
        
        HashSet<string> seen = [];

        for (int row = 0; row < numGridRows; row++) {
            for (int col = 0; col < numGridCols; col++) {
                char value = grid[row * numGridCols + col];
                
                if (value < '1' || value > '0' + numGridRows) return false;
                
                if (
                    !seen.Add($"{value} in row {row}") ||
                    !seen.Add($"{value} in col {col}") ||
                    !seen.Add($"{value} in box {row / numBoxRows}:{col / numBoxCols}")
                ) return false;
            }
        }
        
        return true;
    }
}