using Sudoku.Core.Models;
using Sudoku.Core.Records;

namespace Sudoku.DataAccess.Services;

public class SnapshotManager
{
    private readonly LinkedList<Snapshot> _history = [];
    private LinkedListNode<Snapshot> _current;
    private const int MaxSize = 50;

    public SnapshotManager() {
        _current = _history.AddLast(new Snapshot());
    }

    public void Record(Snapshot snapshot) {
        while (_current.Next is not null) {
            _history.RemoveLast();
        }
        
        _current = _history.AddLast(snapshot);

        if (_history.Count > MaxSize) {
            _history.RemoveFirst();
        }
    }
    
    public Snapshot Undo() {
        var snapshot = _current.Value;
        
        if (_current.Previous is not null) {
            _current = _current.Previous;
        }
        
        return snapshot;
    }

    public Snapshot Redo() {
        if (_current.Next is not null) {
            _current = _current.Next;
        }
        
        return _current.Value;
    }
}