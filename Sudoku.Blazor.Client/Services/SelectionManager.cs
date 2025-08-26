using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Blazor.Client.Services;

public class SelectionManager(PuzzleSession currentSession)
{
    private PuzzleSession _currentSession = currentSession;
    
    public void SetCurrentSession(PuzzleSession newSession) {
        _currentSession = newSession;
    }
    
    public bool IsCellSelected(Cell cell) {
        return _currentSession.SelectedCells.Contains(cell);
    }

    public void SelectCell(Cell cell) {
        _currentSession.SelectedCells.Add(cell);
    }

    public void DeselectCell(Cell cell) {
        _currentSession.SelectedCells.Remove(cell);
    }

    public void DeselectAllCells() {
        _currentSession.SelectedCells.Clear();
    }
    
    public void FilterSelection(Cell cell) {
        switch (_currentSession.SelectionMode) {
            case SelectionMode.Regular:
                if (_currentSession.SelectedCells.Contains(cell) && _currentSession.SelectedCells.Count == 1) {
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
            _currentSession.SelectionMode = IsCellSelected(cell) ? SelectionMode.Delete : SelectionMode.Select;
        }
        
        FilterSelection(cell);
    }
    
    public void HandleMouseUp() {
        _currentSession.SelectionMode = SelectionMode.Regular;
    }

    public void HandleMouseEnter(Cell cell) {
        if (_currentSession.SelectionMode == SelectionMode.Regular) {
            _currentSession.SelectionMode = SelectionMode.Select;
        }
        
        FilterSelection(cell);
    }

    public void TraverseGrid(Grid grid, int deltaRow, int deltaCol) {
        var lastCellSelected = _currentSession.SelectedCells.Last();
        
        int newRow = (lastCellSelected.Row + deltaRow + grid.NumCols) % grid.NumCols;
        int newCol = (lastCellSelected.Col + deltaCol + grid.NumRows) % grid.NumRows;
        
        DeselectAllCells();
        SelectCell(grid.GetCell(newRow, newCol));
    }
}