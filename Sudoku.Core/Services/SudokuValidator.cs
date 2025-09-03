namespace Sudoku.Core.Services;

public class SudokuValidator
{
    /// <summary>
    /// Validates a Sudoku grid by ensuring:
    /// 1. Grid size matches the specified dimensions.
    /// 2. All values are within the expected numeric range.
    /// 3. No duplicate values exist within any row, column, or box.
    /// </summary>
    public bool IsValid(List<char> grid, int numGridRows, int numGridCols, int numBoxRows, int numBoxCols) {
        if (numGridRows * numGridCols != grid.Count) return false;
        
        // Used for ensuring there are no duplicate values in any row, column, or box.
        HashSet<string> seen = [];

        for (int row = 0; row < numGridRows; row++) {
            for (int col = 0; col < numGridCols; col++) {
                char value = grid[row * numGridCols + col];
                
                if (value < '1' || value > '0' + numGridRows) return false;
                
                // If unable to add to seen, it already exists, meaning there is a duplicate value.
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