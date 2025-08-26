namespace Sudoku.Core.Models;

public class Cell(int row, int col)
{
    public int Row { get; set; } = row;
    public int Col { get; set; } = col;
    public char Value { get; set; } = '\0';
    public bool IsGiven { get; set; }
    public PencilMarks PencilMarks { get; set; } = new();

    public void CopyFrom(Cell other) {
        Row = other.Row;
        Col = other.Col;
        Value = other.Value;
        IsGiven = other.IsGiven;
        PencilMarks = other.PencilMarks;
    }
}