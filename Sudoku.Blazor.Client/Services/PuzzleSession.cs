using Blazored.LocalStorage;
using Sudoku.Core.Models;
using Sudoku.DataAccess.Models;
using Sudoku.Core.Services;

namespace Sudoku.Blazor.Client.Services;

/// <summary>
/// Manages the state of the active puzzle session. 
/// Provides selection handling, undo/redo functionality, and persistence of the puzzle grid to local storage.
/// Also tracks whether the puzzle is solved.
/// </summary>
public class PuzzleSession(ILocalStorageService localStorage)
{
    public PuzzleModel Puzzle { get; set; }
    public Grid Grid { get; set; }

    public InputManager InputManager { get; set; }
    public SelectionManager SelectionManager { get; set; }
    public UndoRedoService UndoRedoService { get; set; }

    private SudokuValidator _sudokuValidator = new();
    public bool IsSolved { get; set; }
    
    /// <summary>
    /// Initializes the puzzle session.
    /// If possible, it gets the grid values from local storage, otherwise it builds a new grid.
    /// </summary>
    public async Task Initialize(PuzzleModel puzzle) {
        Puzzle = puzzle;
        
        var grid = await LoadGrid(puzzle.Id);
        
        if (grid is null) {
            Grid = new Grid(puzzle.NumRows, puzzle.NumCols);
            Grid.LoadPuzzle(puzzle.Grid);
            await SaveGrid(puzzle.Id, Grid);
        }
        else {
            Grid = grid;
        }

        // Solve initially as it may have been previously solved and saved in local storage.
        Solve();
        
        SelectionManager = new SelectionManager();
        UndoRedoService = new UndoRedoService(Grid, SelectionManager);
        InputManager = new InputManager(Grid, SelectionManager, UndoRedoService);
    
        // Subscribe to event in InputManager. Triggered whenever a cell in updated.
        InputManager.CellUpdated += OnCellUpdated;
    }

    /// <summary>
    /// Reset puzzle sessions without destroying references.
    /// </summary>
    public async Task ClearSession() {
        await DeleteGrid(Puzzle.Id);
        Grid.Clear();
        
        IsSolved = false;
        
        await SaveGrid(Puzzle.Id, Grid);
    }
    
    public void Solve() {
        List<char> grid = Grid.GetCells().Select(cell => cell.Value).ToList();
        IsSolved = _sudokuValidator.IsValid(grid, Grid.NumRows, Grid.NumCols, 3, 3);
    }

    private async Task OnCellUpdated() {
        try {
            await SaveGrid(Puzzle.Id, Grid);
        }
        catch (Exception ex) {
            await Console.Error.WriteLineAsync($"Failed to save puzzle: {ex}");
        }
    }

    private async Task SaveGrid(string id, Grid grid) {
        await localStorage.SetItemAsync($"puzzle_{id}", grid);
    }
    
    private async Task<Grid?> LoadGrid(string id) {
        return await localStorage.GetItemAsync<Grid>($"puzzle_{id}");
    }
    
    private async Task DeleteGrid(string id) {
        await localStorage.RemoveItemAsync($"puzzle_{id}");
    }
}