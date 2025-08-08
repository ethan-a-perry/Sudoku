namespace Sudoku.DataAccess.Enums;

/// <summary>
/// Specifies the relative direction of a neighboring cell in relation to a given cell.
/// </summary>
public enum Direction
{
    Top,
    Right,
    Bottom,
    Left,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}