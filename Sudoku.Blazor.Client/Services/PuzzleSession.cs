using Blazored.LocalStorage;
using Sudoku.Core.Models;
using Sudoku.DataAccess.Models;
using Sudoku.DataAccess.Services;

namespace Sudoku.Blazor.Client.Services;

public class PuzzleSession(ILocalStorageService localStorage)
{
    public PuzzleModel Puzzle { get; set; }
    public Grid Grid { get; set; }

    public InputManager InputManager { get; set; }
    public SelectionManager SelectionManager { get; set; }
    public UndoRedoService UndoRedoService { get; set; }
    
    private SudokuSolver SudokuSolver { get; set; } = new();
    public bool IsSolved { get; set; }
    
    public async Task InitializeAsync(PuzzleModel puzzle) {
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

        Solve();
        
        SelectionManager = new SelectionManager();
        UndoRedoService = new UndoRedoService(Grid, SelectionManager);
        InputManager = new InputManager(Grid, SelectionManager, UndoRedoService);
    
        InputManager.CellUpdated += OnCellUpdated;
    }

    public async Task ClearSession() {
        await DeleteGrid(Puzzle.Id);
        Grid.Clear();
        
        IsSolved = false;
        
        await SaveGrid(Puzzle.Id, Grid);
    }
    
    public void Solve() {
        IsSolved = SudokuSolver.IsSolved(Grid);
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