using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sudoku.Blazor.Client.Services;
using Sudoku.Core.Models;
using Sudoku.DataAccess.Services;

namespace Sudoku.Blazor.Client.Components.Shared;

public partial class SudokuGrid : ComponentBase
{
    [Inject] private Grid Grid { get; set; }
    [Inject] private UndoRedoService UndoRedoService { get; set; }
    [Inject] private SelectionManager SelectionManager { get; set; }
    [Inject] private InputManager InputManager { get; set; }
    private SudokuSolver SudokuSolver { get; set; } = new();
    private bool IsMouseDown { get; set; }
    private bool IsShiftKeyDown { get; set; }
    
    private void Solve() {
        Console.WriteLine(SudokuSolver.IsSolved(Grid.Cells) ? "Solved" : "Not solved");
    }
    
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

        InputManager.FilterKeyboardEvent(e);
    }
    
    private void OnKeyUp(KeyboardEventArgs e) {
        IsShiftKeyDown = e.ShiftKey;
    }
}