using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sudoku.Blazor.Services;
using Sudoku.Core.Models;
using Sudoku.DataAccess.Services;

namespace Sudoku.Blazor.Components.Shared;

public partial class SudokuGrid : ComponentBase
{
    [Inject] private Grid Grid { get; set; }
    [Inject] private UndoRedoService UndoRedoService { get; set; }
    [Inject] private SelectionManager SelectionManager { get; set; }
    [Inject] private InputManager InputManager { get; set; }
    private bool IsMouseDown { get; set; }
    private bool IsShiftKeyDown { get; set; }
    
    private void OnMouseDown(MouseEventArgs e, int row, int col) {
        // If the user right-clicks or opens the context menu, restrict grid interactivity.
        if (e.Button != 0) return;
        IsMouseDown = true;
        
        SelectionManager.HandleMouseDown(Grid.Cells[row, col], IsShiftKeyDown);
    }
    
    private void OnMouseUp() {
        IsMouseDown = false;
        SelectionManager.HandleMouseUp();
    }
    
    /// <summary>
    /// Lets the user toggle a cell whenever they are dragging their mouse.
    /// </summary>
    private void OnMouseEnter(int row, int col) {
        if (!IsMouseDown) return;
        SelectionManager.HandleMouseEnter(Grid.Cells[row, col]);
    }
    
    private void OnKeyDown(KeyboardEventArgs e) {
        // If the shiftkey is pressed, Select or Delete mode can be activated.
        IsShiftKeyDown = e.ShiftKey;

        // Initiate undo or redo by checking Ctrl (Windows) and Cmd (Mac) keys
        switch (e.Key.ToLowerInvariant()) {
            // Ctrl+Y or Cmd+Y
            case "y" when e.CtrlKey || e.MetaKey:
                UndoRedoService.Redo();
                return;
            // Ctrl+Z or Cmd+Z
            case "z" when e.CtrlKey || e.MetaKey && !e.ShiftKey:
                UndoRedoService.Undo();
                return;
            // Cmd+Shift+Z
            case "z" when e.MetaKey && e.ShiftKey:
                UndoRedoService.Redo();
                return;
        }
        
        InputManager.FilterInput(e.Key);
    }
    
    private void OnKeyUp(KeyboardEventArgs e) {
        IsShiftKeyDown = e.ShiftKey;
    }
}