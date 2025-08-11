using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sudoku.DataAccess.Data;
using Sudoku.DataAccess.Enums;
using Sudoku.DataAccess.Models;

namespace Sudoku.Blazor.Components.Shared;

public partial class SudokuGrid : ComponentBase
{
    [Parameter] public GridModel Grid { get; set; } = new();
    private bool IsMouseDown { get; set; }
    private bool IsShiftKeyDown { get; set; }
    
    /// <summary>
    /// Checks when a mouse is pressed down, not necessarily clicked.
    /// </summary>
    private void OnMouseDown(MouseEventArgs e, int row, int col) {
        // If the user right-clicks or opens the context menu, restrict grid interactivity.
        if (e.Button != 0) return;
        
        IsMouseDown = true;
        
        // See GridMode.cs for info about the various grid modes.
        if (IsShiftKeyDown) {
            Grid.Mode = Grid.Cells[row, col].IsSelected switch {
                true => GridMode.Delete,
                false => GridMode.Select
            };
        }
        
        // Figure out of a cell is selected or deselected.
        Grid.SortSelection(Grid.Cells[row, col]);
    }
    
    private void OnMouseUp() {
        IsMouseDown = false;
        Grid.Mode = GridMode.Regular;
    }
    
    /// <summary>
    /// Lets the user toggle a cell whenever they are dragging their mouse.
    /// </summary>
    private void OnMouseEnter(int row, int col) {
        if (!IsMouseDown) return;

        if (Grid.Mode == GridMode.Regular) {
            Grid.Mode = GridMode.Select;
        }
        
        Grid.SortSelection(Grid.Cells[row, col]);
    }

    private void OnKeyDown(KeyboardEventArgs e) {
        // If the shiftkey is pressed, Select or Delete mode can be activated.
        IsShiftKeyDown = e.ShiftKey;

        switch (e.Key) {
            case { Length: 1 }:
                Grid.SetCell(e.Key.ToUpper()[0]);
                break;
            case "Backspace":
                Grid.SetCell('\0');
                break;
        }
    }

    private void OnKeyUp(KeyboardEventArgs e) {
        IsShiftKeyDown = e.ShiftKey;
    }
}