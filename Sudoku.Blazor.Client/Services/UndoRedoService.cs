using Sudoku.Core.Models;
using Sudoku.Core.Records;
using Sudoku.Core.Services;

namespace Sudoku.Blazor.Client.Services;

/// <summary>
/// Keeps track of sudoku moves to offer undo/redo functionality.
/// </summary>
public class UndoRedoService(Grid grid, SelectionManager selectionManager)
{
    public SnapshotManager SnapshotManager { get; set; } = new();
    public IEnumerable<Cell> EditableCells => selectionManager.EditableCells;
    
    /// <summary>
    /// Takes a snapshot before and after a change is made to the grid.
    /// Snapshots are only taken of the editable cells affected by a move, not the whole grid.
    /// </summary>
    public void RecordSnapshot(Action applyCellChanges) {
        var before = EditableCells.Select(CellState.FromCell).ToList();
        
        applyCellChanges();
        
        var after = EditableCells.Select(CellState.FromCell).ToList();

        SnapshotManager.Record(new Snapshot(before, after));
    }
    
    /// <summary>
    /// Restores the cells affected by a move according to a previously saved snapshot. 
    /// </summary>
    public void RestoreSnapshot(List<CellState> cellStates) {
        // Deselect all cells first
        selectionManager.DeselectAllCells();
        
        foreach (var cellState in cellStates) {
            var cell = grid.GetCell(cellState.Row, cellState.Col);
            char value = cellState.Value;
            
            selectionManager.SelectCell(cell);
            grid.SetDigit(cell, value);
        }
    }
	
    public void Undo() {
        var snapshot = SnapshotManager.Undo();
        RestoreSnapshot(snapshot.Before);
    }
	
    public void Redo() {
        var snapshot = SnapshotManager.Redo();
        RestoreSnapshot(snapshot.After);
    }
}