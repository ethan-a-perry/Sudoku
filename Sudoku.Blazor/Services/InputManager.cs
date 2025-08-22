using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Blazor.Services;

public class InputManager(SelectionManager selectionManager)
{
    public InputMode Mode { get; set; } = InputMode.Digit;

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
        switch (Mode) {
            case InputMode.Digit:
                if (selectionManager.EditableCells.All(c => c.Value == input)) {
                    grid.UnsetDigit(selectionManager.EditableCells);
                }
                else {
                    grid.SetDigit(selectionManager.EditableCells, input);
                }
                break;
            case InputMode.CornerPencilMark:
                if (selectionManager.EditableCells.All(c => c.PencilMarks.Corner.Contains(input))) {
                    grid.UnsetCornerPencilMark(selectionManager.EditableCells, input);
                }
                else {
                    grid.SetCornerPencilMark(selectionManager.EditableCells, input);
                }
                break;
            case InputMode.CenterPencilMark:
                if (selectionManager.EditableCells.All(c => c.PencilMarks.Center.Contains(input))) {
                    grid.UnsetCenterPencilMark(selectionManager.EditableCells, input);
                }
                else {
                    grid.SetCenterPencilMark(selectionManager.EditableCells, input);
                }
                break;
        }
    }

    public void HandleUnset(Grid grid) {
        // If any cells have Digits, remove then return
        if (selectionManager.EditableCells.Any(c => c.Value is not '\0')) {
            grid.UnsetDigit(selectionManager.EditableCells);
            return;
        }
        
        // If any of the cells have corner pencil marks,
        // check if the input mode is not a CenterPencilMark or if no center pencil marks exist.
        // If these pass, remove corner pencil marks
        if (selectionManager.EditableCells.Any(c => c.PencilMarks.Corner.Count > 0)) {
            if (Mode is not InputMode.CenterPencilMark || selectionManager.EditableCells.Any(c => c.PencilMarks.Center.Count == 0)) { 
                grid.UnsetCornerPencilMarks(selectionManager.EditableCells);
                return;
            }
        }
        
        // Remove all center pencil marks
        grid.UnsetCenterPencilMarks(selectionManager.EditableCells);
    }
}