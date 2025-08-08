namespace Sudoku.DataAccess.Enums;

/// <summary>
/// Ensures a selected cell is properly styled by applying borders where necessary.
/// </summary>
[Flags]
public enum Borders
{
    None = 1 << 0,
    
    Top = 1 << 1,
    Right = 1 << 2,
    Bottom = 1 << 3,
    Left = 1 << 4,
    AllBorders = Top | Right | Bottom | Left,
    
    // Corner borders are only used when the outline makes an edge-turn.
    TopLeftCorner = 1 << 5,
    TopRightCorner = 1 << 6,
    BottomLeftCorner = 1 << 7,
    BottomRightCorner = 1 << 8,
    AllCorners = TopLeftCorner | TopRightCorner | BottomLeftCorner | BottomRightCorner,
    
    All =  AllBorders | AllCorners
}