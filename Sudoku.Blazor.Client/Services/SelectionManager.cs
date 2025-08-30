using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.DataAccess;

namespace Sudoku.Blazor.Client.Services;

public class SelectionManager
{
    public SelectionMode SelectionMode { get; set; } = SelectionMode.Regular;
    public LinkedHashSet<Cell> SelectedCells { get; set; } = [];
    public IEnumerable<Cell> EditableCells => SelectedCells.Where(c => !c.IsGiven);
    
    public bool IsCellSelected(Cell cell) => SelectedCells.Contains(cell);
    public void SelectCell(Cell cell) => SelectedCells.Add(cell);
    public void DeselectCell(Cell cell) => SelectedCells.Remove(cell);
    public void DeselectAllCells() => SelectedCells.Clear();
    
    public void FilterSelection(Cell cell) {
        switch (SelectionMode) {
            case SelectionMode.Regular:
                if (SelectedCells.Contains(cell) && SelectedCells.Count == 1) {
                    DeselectCell(cell);
                }
                else {
                    DeselectAllCells();
                    SelectCell(cell);
                }
                break;
            case SelectionMode.Select:
                SelectCell(cell);
                break;
            case SelectionMode.Delete:
                DeselectCell(cell);
                break;
        }
    }
    
    public void HandleMouseDown(Cell cell, bool isShiftKeyDown = false) {
        // See GridMode.cs for info about the various grid modes.
        if (isShiftKeyDown) {
            SelectionMode = IsCellSelected(cell) ? SelectionMode.Delete : SelectionMode.Select;
        }
        
        FilterSelection(cell);
    }
    
    public void HandleMouseUp() {
        SelectionMode = SelectionMode.Regular;
    }

    public void HandleMouseEnter(Cell cell) {
        if (SelectionMode == SelectionMode.Regular) {
            SelectionMode = SelectionMode.Select;
        }
        
        FilterSelection(cell);
    }

    public void TraverseGrid(Grid grid, int deltaRow, int deltaCol) {
        var lastCellSelected = SelectedCells.Last();
        
        int newRow = (lastCellSelected.Row + deltaRow + grid.NumCols) % grid.NumCols;
        int newCol = (lastCellSelected.Col + deltaCol + grid.NumRows) % grid.NumRows;
        
        DeselectAllCells();
        SelectCell(grid.GetCell(newRow, newCol));
    }
}