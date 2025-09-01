using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.DataAccess;
using Sudoku.DataAccess.Models;
using Sudoku.DataAccess.Services;

namespace Sudoku.Blazor.Client.Services;

public class PuzzleSession
{
    public PuzzleModel Puzzle { get; set; }
    public Grid Grid { get; set; }

    public InputManager InputManager { get; set; }
    public SelectionManager SelectionManager { get; set; }
    public UndoRedoService UndoRedoService { get; set; }

    public PuzzleSession(PuzzleModel puzzle) {
        Puzzle = puzzle;
        
        Grid = new Grid(puzzle.NumRows, puzzle.NumCols);
        Grid.LoadPuzzle(puzzle.Grid);
        
        SelectionManager = new SelectionManager();
        UndoRedoService = new UndoRedoService(Grid, SelectionManager);
        InputManager = new InputManager(Grid, SelectionManager, UndoRedoService);
    }
}