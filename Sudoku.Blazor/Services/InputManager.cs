using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.Core.Records;
using Sudoku.DataAccess.Services;

namespace Sudoku.Blazor.Services;

public class InputManager(SelectionManager selectionManager)
{
    public InputMode Mode { get; set; } = InputMode.Digit;
    public SnapshotManager SnapshotManager { get; set; } = new();

    public void FilterInput(Grid grid, string input) {
        switch (input) {
            case { Length: 1 }:
                HandleSet(grid, input.ToUpper()[0]);
                break;
            case "Backspace":
                HandleUnset(grid);
                break;
            case "Tab":
                Mode = Mode switch {
                    InputMode.Digit => InputMode.CenterPencilMark,
                    InputMode.CenterPencilMark => InputMode.CornerPencilMark,
                    InputMode.CornerPencilMark => InputMode.Digit,
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;
        }
    }

    public void HandleSet(Grid grid, char input) {
        var editableCells = selectionManager.EditableCells;
        RecordSnapshot(() => {
            switch (Mode) {
                case InputMode.Digit:
                    if (editableCells.All(c => c.Value == input)) {
                        grid.UnsetDigit(editableCells);
                    }
                    else {
                        grid.SetDigit(editableCells, input);
                    }
                    break;
                case InputMode.CornerPencilMark:
                    if (editableCells.All(c => c.PencilMarks.Corner.Contains(input))) {
                        grid.UnsetCornerPencilMark(editableCells, input);
                    }
                    else {
                        grid.SetCornerPencilMark(editableCells, input);
                    }
                    break;
                case InputMode.CenterPencilMark:
                    if (editableCells.All(c => c.PencilMarks.Center.Contains(input))) {
                        grid.UnsetCenterPencilMark(editableCells, input);
                    }
                    else {
                        grid.SetCenterPencilMark(editableCells, input);
                    }
                    break;
            }
        });
    }

    public void HandleUnset(Grid grid) {
        var editableCells = selectionManager.EditableCells;
        RecordSnapshot(() => {
            // If any cells have Digits, remove then return
            if (editableCells.Any(c => c.Value is not '\0')) {
                grid.UnsetDigit(editableCells);
                return;
            }
        
            // If any of the cells have corner pencil marks,
            // check if the input mode is not a CenterPencilMark or if no center pencil marks exist.
            // If these pass, remove corner pencil marks
            if (editableCells.Any(c => c.PencilMarks.Corner.Count > 0)) {
                if (Mode is not InputMode.CenterPencilMark || editableCells.Any(c => c.PencilMarks.Center.Count == 0)) { 
                    grid.UnsetCornerPencilMarks(editableCells);
                    return;
                }
            }
        
            // Remove all center pencil marks
            grid.UnsetCenterPencilMarks(selectionManager.EditableCells);
        });
    }
    
    public void RecordSnapshot(Action applyInput) {
        var before = selectionManager.EditableCells.Select(CellState.FromCell).ToList();
        
        applyInput();
        
        var after = selectionManager.EditableCells.Select(CellState.FromCell).ToList();

        SnapshotManager.Record(new Snapshot(before, after));
    }
    
    public void RestoreSnapshot(Grid grid, List<CellState> cellStates) {
        selectionManager.DeselectAllCells();
        
        foreach (var cellState in cellStates) {
            var cell = grid.Cells[cellState.Row, cellState.Col];
            char value = cellState.Value;
            
            selectionManager.SelectCell(cell);
            grid.SetDigit(cell, value);
        }
    }
	
    public void Undo(Grid grid) {
        var snapshot = SnapshotManager.Undo();
        RestoreSnapshot(grid, snapshot.Before);
    }
	
    public void Redo(Grid grid) {
        var snapshot = SnapshotManager.Redo();
        RestoreSnapshot(grid, snapshot.After);
    }
}