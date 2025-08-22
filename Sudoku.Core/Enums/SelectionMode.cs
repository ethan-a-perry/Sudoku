namespace Sudoku.Core.Enums;

/// <summary>
/// Specifies various grid states which dictate how cells are toggled.
/// </summary>
/// <remarks>
/// Regular: Anytime a cell is selected, it deselects all other cells and toggles the current selection.
/// Select: Only unselected cells may be toggled.
/// Delete: Only selected cells may be toggled.
/// </remarks>
public enum SelectionMode
{
    Regular,
    Select,
    Delete
}