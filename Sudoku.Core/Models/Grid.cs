namespace Sudoku.Core.Models;

public class Grid
{
    public Cell[,] Cells { get; set; }

    public Grid(int rows, int cols) {
        Cells = new Cell[rows, cols];

        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                Cells[row, col] = new Cell(row, col);
            }
        }
    }
    
    public void InitializePuzzle(string[,] grid) {
        for (int row = 0; row < grid.GetLength(0); row++) {
            for (int col = 0; col < grid.GetLength(1); col++) {
                // s is your string of length 1 with a digit '1' to '9'
                if (grid[row, col] is [>= '1' and <= '9'] s) {
                    Cells[row, col].Value = s[0];
                    Cells[row, col].IsGiven = true;
                }
                // If no digit is given, set the value to null
                else {
                    Cells[row, col].Value = '\0';
                    Cells[row, col].IsGiven = false;
                }
            }
        }
    }
    
    public void SetDigit(IEnumerable<Cell> cells, char value) {
        foreach (Cell cell in cells) {
            cell.Value = value;
        }
    }

    public void UnsetDigit(IEnumerable<Cell> cells) {
        foreach (Cell cell in cells) {
            cell.Value = '\0';
        }
    }
    
    public void SetCornerPencilMark(IEnumerable<Cell> cells, char value) {
        foreach (Cell cell in cells) {
            cell.PencilMarks.Corner.Add(value);
        }
    }
    
    public void UnsetCornerPencilMark(IEnumerable<Cell> cells, char value) {
        foreach (Cell cell in cells) {
            cell.PencilMarks.Corner.Remove(value);
        }
    }

    public void UnsetCornerPencilMarks(IEnumerable<Cell> cells) {
        foreach (Cell cell in cells) {
            cell.PencilMarks.Corner.Clear();
        }
    }
    
    public void SetCenterPencilMark(IEnumerable<Cell> cells, char value) {
        foreach (Cell cell in cells) {
            cell.PencilMarks.Center.Add(value);
        }
    }
    
    public void UnsetCenterPencilMark(IEnumerable<Cell> cells, char value) {
        foreach (Cell cell in cells) {
            cell.PencilMarks.Center.Remove(value);
        }
    }

    public void UnsetCenterPencilMarks(IEnumerable<Cell> cells) {
        foreach (Cell cell in cells) {
            cell.PencilMarks.Center.Clear();
        }
    }
}