using Microsoft.AspNetCore.Components.Web;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;

namespace Sudoku.Blazor.Client.Services;

public class InputManager(Grid grid, SelectionManager selectionManager, UndoRedoService undoRedoService)
{
    public InputMode InputMode { get; set; } = InputMode.Normal;
    public IEnumerable<Cell> EditableCells => selectionManager.EditableCells;
    

    private bool _isMouseDown;
    private bool _isShiftKeyDown;

    public event EventHandler? GridUpdate;

    public void FilterKeyboardEvent(KeyboardEventArgs e) {
        switch (e.Key.ToLowerInvariant()) {
            // Ctrl+Y or Cmd+Y
            case "y" when e.CtrlKey || e.MetaKey:
                Redo();
                return;
            // Ctrl+Z or Cmd+Z
            case "z" when (e.CtrlKey || e.MetaKey) && !e.ShiftKey:
                Undo();
                return;
            // Cmd+Shift+Z
            case "z" when e.MetaKey && e.ShiftKey:
                Redo();
                return;
        }
        
        switch (e.Key) {
            // When any single character is entered
            case { Length: 1 }:
                HandleSet(e.Key.ToUpper()[0]);
                break;
            case "Backspace":
                HandleUnset();
                break;
            case "Tab":
                InputMode = InputMode switch {
                    InputMode.Normal => InputMode.Center,
                    InputMode.Center => InputMode.Corner,
                    InputMode.Corner => InputMode.Normal,
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;
            case "ArrowUp" or "ArrowRight" or "ArrowDown" or "ArrowLeft":
                // Find grid offset according to the arrow key direction
                var (row, col) = e.Key switch {
                    "ArrowUp" => (-1, 0),
                    "ArrowRight" => (0, 1),
                    "ArrowDown" => (1, 0),
                    "ArrowLeft" => (0, -1),
                    _ => (0, 0)
                };
                
                selectionManager.TraverseGrid(grid, row, col);
                break;
        }
    }

    public void HandleSet(char input) {
        undoRedoService.RecordSnapshot(() => {
            switch (InputMode) {
                case InputMode.Normal:
                    if (EditableCells.All(c => c.Value == input)) {
                        grid.UnsetDigit(EditableCells);
                    }
                    else {
                        grid.SetDigit(EditableCells, input);
                    }
                    break;
                case InputMode.Corner:
                    if (EditableCells.All(c => c.PencilMarks.Corner.Contains(input))) {
                        grid.UnsetCornerPencilMark(EditableCells, input);
                    }
                    else {
                        grid.SetCornerPencilMark(EditableCells, input);
                    }
                    break;
                case InputMode.Center:
                    if (EditableCells.All(c => c.PencilMarks.Center.Contains(input))) {
                        grid.UnsetCenterPencilMark(EditableCells, input);
                    }
                    else { 
                        grid.SetCenterPencilMark(EditableCells, input);
                    }
                    break;
            }
        });
        
        GridUpdate?.Invoke(this, EventArgs.Empty);
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
                if (InputMode is not InputMode.Center || EditableCells.Any(c => c.PencilMarks.Center.Count == 0)) { 
                    grid.UnsetCornerPencilMarks(EditableCells);
                    return;
                }
            }
        
            // Remove all center pencil marks
            grid.UnsetCenterPencilMarks(EditableCells);
        });
        
        GridUpdate?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnMouseDown(MouseEventArgs e, int row, int col) {
        // If the user right-clicks or opens the context menu, restrict grid interactivity.
        if (e.Button != 0) return;
        _isMouseDown = true;
        
        selectionManager.HandleMouseDown(grid.GetCell(row, col), _isShiftKeyDown);
    }
    
    public void OnMouseUp() {
        _isMouseDown = false;
        selectionManager.HandleMouseUp();
    }
    
    /// <summary>
    /// Lets the user toggle a cell whenever they are dragging their mouse.
    /// </summary>
    public void OnMouseEnter(int row, int col) {
        if (!_isMouseDown) return;
        selectionManager.HandleMouseEnter(grid.GetCell(row, col));
    }
    
    public void OnKeyDown(KeyboardEventArgs e) {
        // If the shiftkey is pressed, Select or Delete mode can be activated.
        _isShiftKeyDown = e.ShiftKey;
    
        FilterKeyboardEvent(e);
    }
    
    public void OnKeyUp(KeyboardEventArgs e) {
        _isShiftKeyDown = e.ShiftKey;
    }

    public void SetInputMode(InputMode newMode) {
        InputMode = newMode;
    }
    
    public void Undo() {
        undoRedoService.Undo();
        GridUpdate?.Invoke(this, EventArgs.Empty);
    }

    public void Redo() {
        undoRedoService.Redo();
        GridUpdate?.Invoke(this, EventArgs.Empty);
    }
}