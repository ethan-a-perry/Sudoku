using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Sudoku.Core.Enums;
using Sudoku.Core.Models;
using Sudoku.DataAccess;
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
        
        SelectionManager = new SelectionManager();
        UndoRedoService = new UndoRedoService(Grid, SelectionManager);
        InputManager = new InputManager(Grid, SelectionManager, UndoRedoService);
    
        InputManager.CellUpdated += OnCellUpdated;
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
}