using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sudoku.Blazor.Services;
using Sudoku.Core.Models;
using Sudoku.DataAccess.Services;

namespace Sudoku.Blazor.Components.Shared;

public partial class SudokuGrid : ComponentBase
{
    [Inject] private Grid Grid { get; set; }
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
        InputManager.FilterInput(e.Key);
    }
    
    private void OnKeyUp(KeyboardEventArgs e) {
        IsShiftKeyDown = e.ShiftKey;
    }
}