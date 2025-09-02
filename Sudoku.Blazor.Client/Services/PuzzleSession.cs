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
        
        Grid = new Grid(puzzle.NumRows, puzzle.NumCols);
        Grid.LoadPuzzle(puzzle.Grid);
        
        var grid = await LoadGrid(Puzzle.Id);
        
        if (grid is null) {
            Grid = new Grid(puzzle.NumRows, puzzle.NumCols);
            await SaveGrid(Puzzle.Id, Grid);
        }
        else {
            // Copies data from one cell to the other. Important to preserve the references.
            for (int i = 0; i < Grid.GetCells().Count; i++) {
                Grid.GetCells()[i].CopyFrom(grid.GetCells()[i]);
            }
        }
        
        SelectionManager = new SelectionManager();
        UndoRedoService = new UndoRedoService(Grid, SelectionManager);
        InputManager = new InputManager(Grid, SelectionManager, UndoRedoService);
    
        InputManager.CellUpdated += OnCellUpdated;
    }

    public async Task OnCellUpdated() {
        Console.WriteLine("Cell updated");
        
        try {
            Console.WriteLine("Trying save");
            await SaveGrid(Puzzle.Id, Grid);
            Console.WriteLine("Finished save");
        }
        catch (Exception ex) {
            await Console.Error.WriteLineAsync($"Failed to save puzzle: {ex}");
        }
    }

    public async Task SaveGrid(string id, Grid grid) {
        await localStorage.SetItemAsync($"puzzle_{id}", grid);
    }
    
    public async Task<Grid?> LoadGrid(string id) {
        return await localStorage.GetItemAsync<Grid>($"puzzle_{id}");
    }
}