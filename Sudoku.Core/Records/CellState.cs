using Sudoku.Core.Models;

namespace Sudoku.Core.Records;

public record CellState(int Row, int Col, char Value, SortedSet<char> CornerPencilMarks, SortedSet<char> CenterPencilMarks) 
{
    public static CellState FromCell(Cell cell) {
        return new CellState(
            Row: cell.Row,
            Col: cell.Col,
            Value: cell.Value,
            CornerPencilMarks: new SortedSet<char>(cell.PencilMarks.Corner),
            CenterPencilMarks: new SortedSet<char>(cell.PencilMarks.Center)
        );
    }
}