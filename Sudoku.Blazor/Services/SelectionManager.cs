using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Blazor.Services;

public class SelectionManager
{
    public SelectionMode Mode { get; set; } = SelectionMode.Regular;
    public HashSet<Cell> SelectedCells { get; private set; } = [];
    public IEnumerable<Cell> EditableCells => SelectedCells.Where(c => !c.IsGiven);
    
    public bool IsCellSelected(Cell cell) => SelectedCells.Contains(cell);
    public void SelectCell(Cell cell) => SelectedCells.Add(cell);
    public void DeselectCell(Cell cell) => SelectedCells.Remove(cell);

    public void DeselectAllCells() {
        SelectedCells.Clear();
    }
    
    public void FilterSelection(Cell cell) {
        switch (Mode) {
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
            Mode = IsCellSelected(cell) ? SelectionMode.Delete : SelectionMode.Select;
        }
        
        FilterSelection(cell);
    }
    
    public void HandleMouseUp() {
        Mode = SelectionMode.Regular;
    }

    public void HandleMouseEnter(Cell cell) {
        if (Mode == SelectionMode.Regular) {
            Mode = SelectionMode.Select;
        }
        
        FilterSelection(cell);
    }
}