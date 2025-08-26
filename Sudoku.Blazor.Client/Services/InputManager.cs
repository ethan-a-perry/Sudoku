using Microsoft.AspNetCore.Components.Web;
using Sudoku.Core.Enums;

namespace Sudoku.Blazor.Client.Services;

public class InputManager(PuzzleSession currentSession, SelectionManager selectionManager, UndoRedoService undoRedoService)
{
    private PuzzleSession _currentSession = currentSession;

    private bool _isMouseDown;
    private bool _isShiftKeyDown;

    public event EventHandler? GridUpdate;

    public void SetCurrentSession(PuzzleSession newSession) {
        _currentSession = newSession;
    }

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
                _currentSession.InputMode = _currentSession.InputMode switch {
                    InputMode.Digit => InputMode.CenterPencilMark,
                    InputMode.CenterPencilMark => InputMode.CornerPencilMark,
                    InputMode.CornerPencilMark => InputMode.Digit,
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
                
                selectionManager.TraverseGrid(_currentSession.Grid, row, col);
                break;
        }
    }

    public void HandleSet(char input) {
        undoRedoService.RecordSnapshot(() => {
            switch (_currentSession.InputMode) {
                case InputMode.Digit:
                    if (_currentSession.EditableCells.All(c => c.Value == input)) {
                        _currentSession.Grid.UnsetDigit(_currentSession.EditableCells);
                    }
                    else {
                        _currentSession.Grid.SetDigit(_currentSession.EditableCells, input);
                    }
                    break;
                case InputMode.CornerPencilMark:
                    if (_currentSession.EditableCells.All(c => c.PencilMarks.Corner.Contains(input))) {
                        _currentSession.Grid.UnsetCornerPencilMark(_currentSession.EditableCells, input);
                    }
                    else {
                        _currentSession.Grid.SetCornerPencilMark(_currentSession.EditableCells, input);
                    }
                    break;
                case InputMode.CenterPencilMark:
                    if (_currentSession.EditableCells.All(c => c.PencilMarks.Center.Contains(input))) {
                        _currentSession.Grid.UnsetCenterPencilMark(_currentSession.EditableCells, input);
                    }
                    else {
                        _currentSession.Grid.SetCenterPencilMark(_currentSession.EditableCells, input);
                    }
                    break;
            }
        });
        
        GridUpdate?.Invoke(this, EventArgs.Empty);
    }

    public void HandleUnset() {
        undoRedoService.RecordSnapshot(() => {
            // If any cells have Digits, remove then return
            if (_currentSession.EditableCells.Any(c => c.Value is not '\0')) {
                _currentSession.Grid.UnsetDigit(_currentSession.EditableCells);
                return;
            }
        
            // If any of the cells have corner pencil marks,
            // check if the input mode is not a CenterPencilMark or if no center pencil marks exist.
            // If these pass, remove corner pencil marks
            if (_currentSession.EditableCells.Any(c => c.PencilMarks.Corner.Count > 0)) {
                if (_currentSession.InputMode is not InputMode.CenterPencilMark || _currentSession.EditableCells.Any(c => c.PencilMarks.Center.Count == 0)) { 
                    _currentSession.Grid.UnsetCornerPencilMarks(_currentSession.EditableCells);
                    return;
                }
            }
        
            // Remove all center pencil marks
            _currentSession.Grid.UnsetCenterPencilMarks(_currentSession.EditableCells);
        });
        
        GridUpdate?.Invoke(this, EventArgs.Empty);
    }
    
    public void OnMouseDown(MouseEventArgs e, int row, int col) {
        // If the user right-clicks or opens the context menu, restrict grid interactivity.
        if (e.Button != 0) return;
        _isMouseDown = true;
        
        selectionManager.HandleMouseDown(_currentSession.Grid.GetCell(row, col), _isShiftKeyDown);
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
        selectionManager.HandleMouseEnter(_currentSession.Grid.GetCell(row, col));
    }
    
    public void OnKeyDown(KeyboardEventArgs e) {
        // If the shiftkey is pressed, Select or Delete mode can be activated.
        _isShiftKeyDown = e.ShiftKey;
    
        FilterKeyboardEvent(e);
    }
    
    public void OnKeyUp(KeyboardEventArgs e) {
        _isShiftKeyDown = e.ShiftKey;
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