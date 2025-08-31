using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sudoku.Blazor.Client.Services;
using Sudoku.DataAccess.Data;
using Sudoku.DataAccess.Services;
using Sudoku.DataAccess.Models;

namespace Sudoku.Blazor.Client.Components.Shared;

public partial class SudokuGrid : ComponentBase
{
    [Inject] private PuzzleStorageService PuzzleStorageService { get; set; }
    [Inject] private IPuzzleData PuzzleData { get; set; }
    private List<PuzzleModel> _puzzles = [];
    
    private List<PuzzleSession> _sessions = [];
    private PuzzleSession _currentSession;
    
    private SudokuSolver SudokuSolver { get; set; } = new();
    private bool _isSolved;
    
    protected override async Task OnInitializedAsync() {
        _puzzles = await PuzzleData.GetAllPuzzles();
        
        foreach (var puzzle in _puzzles) {
            _sessions.Add(new PuzzleSession(puzzle));
        }
        
        _currentSession = _sessions[0];
        
        _currentSession.InputManager.GridUpdate += HandleGridUpdate;
        
        if (OperatingSystem.IsBrowser()) {
            await LoadGrid(_puzzles.FirstOrDefault());
        }
    }
    
    private async Task LoadSession(ChangeEventArgs e) {
        var puzzleId = e.Value is not null ? (string)e.Value : string.Empty;
        
        if (string.IsNullOrEmpty(puzzleId)) return;
        
        _currentSession = _sessions.FirstOrDefault(s => s.Puzzle.Id == puzzleId)!;
    
        if (OperatingSystem.IsBrowser()) {
            await LoadGrid(_currentSession.Puzzle);
        }
    }

    private async Task LoadGrid(PuzzleModel puzzle) {
        var grid = await PuzzleStorageService.LoadGrid(puzzle.Id);
        
        if (grid is null) {
            await PuzzleStorageService.SaveGrid(puzzle.Id, _currentSession.Grid);
        }
        else {
            // Copies data from one cell to the other. Important to preserve the references.
            for (int i = 0; i < _currentSession.Grid.GetCells().Count; i++) {
                _currentSession.Grid.GetCells()[i].CopyFrom(grid.GetCells()[i]);
            }
        }
    }
    
    private void Solve() {
        _isSolved = SudokuSolver.IsSolved(_currentSession.Grid);
    }
    
    private async void HandleGridUpdate(object? sender, EventArgs e) {
        try {
            await PuzzleStorageService.SaveGrid(_currentSession.Puzzle.Id, _currentSession.Grid);
        }
        catch (Exception ex) {
            await Console.Error.WriteLineAsync($"Failed to save puzzle: {ex}");
        }
    }

    private async Task Restart() {
        var currentPuzzle = _puzzles.FirstOrDefault(p => p.Id == _currentSession.Puzzle.Id);

        _currentSession = new PuzzleSession(currentPuzzle);
        
        await PuzzleStorageService.SaveGrid(_currentSession.Puzzle.Id, _currentSession.Grid);
    }
    
    public void Dispose() {
        _currentSession.InputManager.GridUpdate -= HandleGridUpdate;
    }
}