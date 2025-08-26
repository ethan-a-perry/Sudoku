using Sudoku.Core.Models;
using Sudoku.Core.Records;

namespace Sudoku.Blazor.Client.Services;

public class UndoRedoService(PuzzleSession currentSession, SelectionManager selectionManager)
{
    private PuzzleSession _currentSession = currentSession;

    public void SetCurrentSession(PuzzleSession newSession) {
        _currentSession = newSession;
    }
    
    public void RecordSnapshot(Action applyInput) {
        var before = _currentSession.EditableCells.Select(CellState.FromCell).ToList();
        
        applyInput();
        
        var after = _currentSession.EditableCells.Select(CellState.FromCell).ToList();

        _currentSession.SnapshotManager.Record(new Snapshot(before, after));
    }
    
    public void RestoreSnapshot(List<CellState> cellStates) {
        selectionManager.DeselectAllCells();
        
        foreach (var cellState in cellStates) {
            var cell = _currentSession.Grid.GetCell(cellState.Row, cellState.Col);
            char value = cellState.Value;
            
            selectionManager.SelectCell(cell);
            _currentSession.Grid.SetDigit(cell, value);
        }
    }
	
    public void Undo() {
        var snapshot = _currentSession.SnapshotManager.Undo();
        RestoreSnapshot(snapshot.Before);
    }
	
    public void Redo() {
        var snapshot = _currentSession.SnapshotManager.Redo();
        RestoreSnapshot(snapshot.After);
    }
}