using Sudoku.Core.Models;
using Sudoku.Core.Records;
using Sudoku.DataAccess.Services;

namespace Sudoku.Blazor.Client.Services;

public class UndoRedoService(Grid grid, SelectionManager selectionManager)
{
    private SnapshotManager SnapshotManager { get; set; } = new();
    
    public void RecordSnapshot(Action applyInput) {
        var before = selectionManager.EditableCells.Select(CellState.FromCell).ToList();
        
        applyInput();
        
        var after = selectionManager.EditableCells.Select(CellState.FromCell).ToList();

        SnapshotManager.Record(new Snapshot(before, after));
    }
    
    public void RestoreSnapshot(List<CellState> cellStates) {
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