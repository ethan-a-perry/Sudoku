using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.Core.Records;
using Sudoku.DataAccess.Services;

namespace Sudoku.Blazor.Services;

public class InputManager(Grid grid, SelectionManager selectionManager, UndoRedoService undoRedoService)
{
    public InputMode Mode { get; set; } = InputMode.Digit;
    public IEnumerable<Cell> EditableCells => selectionManager.EditableCells;

    public void FilterInput(string input) {
        switch (input) {
            case { Length: 1 }:
                HandleSet(input.ToUpper()[0]);
                break;
            case "Backspace":
                HandleUnset();
                break;
            case "Tab":
                Mode = Mode switch {
                    InputMode.Digit => InputMode.CenterPencilMark,
                    InputMode.CenterPencilMark => InputMode.CornerPencilMark,
                    InputMode.CornerPencilMark => InputMode.Digit,
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;
            case "ArrowUp" or "ArrowRight" or "ArrowDown" or "ArrowLeft":
                // Find grid offset according to the arrow key direction
                var (row, col) = input switch {
                    "ArrowUp" => (-1, 0),
                    "ArrowRight" => (0, 1),
                    "ArrowDown" => (1, 0),
                    "ArrowLeft" => (0, -1),
                    _ => (0, 0)
                };
                
                selectionManager.TraverseGrid(grid.Cells, row, col);
                break;
        }
    }

    public void HandleSet(char input) {
        undoRedoService.RecordSnapshot(() => {
            switch (Mode) {
                case InputMode.Digit:
                    if (EditableCells.All(c => c.Value == input)) {
                        grid.UnsetDigit(EditableCells);
                    }
                    else {
                        grid.SetDigit(EditableCells, input);
                    }
                    break;
                case InputMode.CornerPencilMark:
                    if (EditableCells.All(c => c.PencilMarks.Corner.Contains(input))) {
                        grid.UnsetCornerPencilMark(EditableCells, input);
                    }
                    else {
                        grid.SetCornerPencilMark(EditableCells, input);
                    }
                    break;
                case InputMode.CenterPencilMark:
                    if (EditableCells.All(c => c.PencilMarks.Center.Contains(input))) {
                        grid.UnsetCenterPencilMark(EditableCells, input);
                    }
                    else {
                        grid.SetCenterPencilMark(EditableCells, input);
                    }
                    break;
            }
        });
    }

    public void HandleUnset() {
        undoRedoService.RecordSnapshot(() => {
            // If any cells have Digits, remove then return
            if (EditableCells.Any(c => c.Value is not '\0')) {
                grid.UnsetDigit(EditableCells);
                return;
            }
        
            // If any of the cells have corner pencil marks,
            // check if the input mode is not a CenterPencilMark or if no center pencil marks exist.
            // If these pass, remove corner pencil marks
            if (EditableCells.Any(c => c.PencilMarks.Corner.Count > 0)) {
                if (Mode is not InputMode.CenterPencilMark || EditableCells.Any(c => c.PencilMarks.Center.Count == 0)) { 
                    grid.UnsetCornerPencilMarks(EditableCells);
                    return;
                }
            }
        
            // Remove all center pencil marks
            grid.UnsetCenterPencilMarks(EditableCells);
        });
    }
}