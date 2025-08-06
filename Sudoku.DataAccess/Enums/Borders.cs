namespace Sudoku.DataAccess.Enums;

[Flags]
public enum Borders
{
    None = 0,
    Top = 1,
    Right = 2,
    Bottom = 4,
    Left = 8,
    All = Top | Right | Bottom | Left
}