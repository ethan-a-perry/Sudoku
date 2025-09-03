using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.Core.Collections;

namespace Sudoku.Blazor.Client.Services;

/// <summary>
/// Handles any logic related to cell selection in the UI.
/// </summary>
public class SelectionManager
{
    public SelectionMode SelectionMode { get; set; } = SelectionMode.Regular;
    public LinkedHashSet<Cell> SelectedCells { get; set; } = [];
    public IEnumerable<Cell> EditableCells => SelectedCells.Where(c => !c.IsGiven);
    
    public bool IsCellSelected(Cell cell) => SelectedCells.Contains(cell);
    public void SelectCell(Cell cell) => SelectedCells.Add(cell);
    public void DeselectCell(Cell cell) => SelectedCells.Remove(cell);
    public void DeselectAllCells() => SelectedCells.Clear();
    
    /// <summary>
    /// Determines which cells can be selected depending on the selection mode.
    /// </summary>
    public void FilterSelection(Cell cell) {
        switch (SelectionMode) {
            case SelectionMode.Regular:
                // If only one cell is highlighted in the grid, toggle it off.
                if (SelectedCells.Contains(cell) && SelectedCells.Count == 1) {
                    DeselectCell(cell);
                }
                // Otherwise, only highlight new selection.
                else {
                    DeselectAllCells();
                    SelectCell(cell);
                }
                break;
            case SelectionMode.Select:
                // Only allow selections.
                SelectCell(cell);
                break;
            case SelectionMode.Delete:
                // Only allow deselections.
                DeselectCell(cell);
                break;
        }
    }
    
    public void HandleMouseDown(Cell cell, bool isShiftKeyDown = false) {
        // Only toggle allow Select or Delete mode to be toggled if the shift key is being held down.
        if (isShiftKeyDown) {
            // Choose the mode based on whether the cell clicked is active or inactive.
            SelectionMode = IsCellSelected(cell) ? SelectionMode.Delete : SelectionMode.Select;
        }
        
        FilterSelection(cell);
    }
    
    public void HandleMouseUp() {
        // Disable Select and Delete mode.
        SelectionMode = SelectionMode.Regular;
    }

    /// <summary>
    /// Handles mouse enter events while the mouse is held down. 
    /// Used by the InputManager to simulate dragging the mouse across the grid, 
    /// updating the current selection state accordingly.
    /// </summary>
    public void HandleMouseEnter(Cell cell) {
        if (SelectionMode == SelectionMode.Regular) {
            SelectionMode = SelectionMode.Select;
        }
        
        FilterSelection(cell);
    }

    /// <summary>
    /// With the arrow keys, the user can traverse around the grid, wrapping at edges.
    /// </summary>
    public void TraverseGrid(Grid grid, int deltaRow, int deltaCol) {
        // Find last selected cell to navigate from if the user has made a multi-selection.
        var lastCellSelected = SelectedCells.Last();
        
        // Wrap at edges.
        int newRow = (lastCellSelected.Row + deltaRow + grid.NumCols) % grid.NumCols;
        int newCol = (lastCellSelected.Col + deltaCol + grid.NumRows) % grid.NumRows;
        
        DeselectAllCells();
        SelectCell(grid.GetCell(newRow, newCol));
    }
}