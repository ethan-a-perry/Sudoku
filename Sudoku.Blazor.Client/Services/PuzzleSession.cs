using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.DataAccess;
using Sudoku.DataAccess.Models;
using Sudoku.DataAccess.Services;

namespace Sudoku.Blazor.Client.Services;

public class PuzzleSession
{
    public string Id { get; set; }
    public Grid Grid { get; }
    
    public SnapshotManager SnapshotManager { get; set; } = new();
    
    public InputMode InputMode { get; set; } = InputMode.Digit;
    public SelectionMode SelectionMode { get; set; } = SelectionMode.Regular;
    
    public LinkedHashSet<Cell> SelectedCells { get; set; } = [];
    public IEnumerable<Cell> EditableCells => SelectedCells.Where(c => !c.IsGiven);

    public PuzzleSession(PuzzleModel puzzle) {
        Id = puzzle.Id;
        
        Grid = new Grid(puzzle.NumRows, puzzle.NumCols);
        Grid.LoadPuzzle(puzzle.Grid);
    }
}