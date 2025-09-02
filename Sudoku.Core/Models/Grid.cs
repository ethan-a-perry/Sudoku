namespace Sudoku.Core.Models;

public class Grid
{
    public int NumRows { get; set; }
    public int NumCols { get; set; }
    public List<Cell> Cells { get; set; } = [];

    public Grid() {
        NumRows = 9;
        NumCols = 9;
        
        InitializePuzzle(NumRows, NumCols);
    }

    public Grid(int numRows, int numCols) {
        NumRows = numRows;
        NumCols = numCols;
        
        InitializePuzzle(numRows, numCols);
    }

    public void InitializePuzzle(int numRows, int numCols) {
        for (int row = 0; row < numRows; row++) {
            for (int col = 0; col < numCols; col++) {
                Cells.Add(new Cell(row, col));
            }
        }
    }
    
    public void LoadPuzzle(List<string> grid) {
        for (int row = 0; row < NumRows; row++) {
            for (int col = 0; col < NumCols; col++) {
                // s is your string of length 1 with a digit '1' to '9'
                if (grid[row * 9 + col] is [>= '1' and <= '9'] s) {
                    GetCell(row, col).Value = s[0];
                    GetCell(row, col).IsGiven = true;
                }
                // If no digit is given, set the value to null
                else {
                    GetCell(row, col).Value = '\0';
                    GetCell(row, col).IsGiven = false;
                }
            }
        }
    }
    
    public List<Cell> GetCells() => Cells;
    
    public Cell GetCell(int row, int col) {
        if (row < 0 || row >= 9 || col < 0 || col >= 9) {
            throw new ArgumentOutOfRangeException("Row or column is out of bounds.");
        }
        
        return Cells[row * 9 + col];
    }
    
    public void SetDigit(IEnumerable<Cell> cells, char value) {
        foreach (Cell cell in cells) {
            cell.Value = value;
        }
    }
    
    public void SetDigit(Cell cell, char value) {
        cell.Value = value;
    }

    public void UnsetDigit(IEnumerable<Cell> cells) {
        foreach (Cell cell in cells) {
            cell.Value = '\0';
        }
    }
    
    public void SetCornerPencilMark(IEnumerable<Cell> cells, char value) {
        foreach (Cell cell in cells) {
            if (cell.Value is '\0') {
                cell.PencilMarks.Corner.Add(value);
            }
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
            if (cell.Value is '\0') {
                cell.PencilMarks.Center.Add(value);
            }
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

    public void Clear() {
        foreach (var cell in Cells.Where(cell => !cell.IsGiven)) {
            cell.Value = '\0';
            cell.PencilMarks.Corner.Clear();
            cell.PencilMarks.Center.Clear();
        }
    }
}