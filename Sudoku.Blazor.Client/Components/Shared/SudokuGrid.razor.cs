using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sudoku.Blazor.Client.Services;
using Sudoku.DataAccess.Data;
using Sudoku.DataAccess.Models;

namespace Sudoku.Blazor.Client.Components.Shared;

public partial class SudokuGrid : ComponentBase
{
    [Inject] IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private PuzzleSessionFactory PuzzleSessionFactory { get; set; }
    [Inject] private IPuzzleData PuzzleData { get; set; }

    private List<PuzzleModel> _puzzles = [];
    
    private List<PuzzleSession> _sessions = [];
    private PuzzleSession? _currentSession;

    private string _activeDialog = string.Empty;

    protected override async Task OnInitializedAsync() {
        // Get available puzzles from data access.
        _puzzles = await PuzzleData.GetAllPuzzles();
        
        // Create a session for each playable puzzle.
        foreach (var puzzle in _puzzles) {
            var newSession = await PuzzleSessionFactory.Create(puzzle);
            _sessions.Add(newSession);
        }
        
        _currentSession = _sessions[0];
        
        
        // Dispatch a global 'sudokuAppReady' event to signal that the grid has finished initializing.
        await JSRuntime.InvokeVoidAsync("eval", "window.dispatchEvent(new Event('sudokuAppReady'));");
    }
    
    private void LoadSession(ChangeEventArgs e) {
        var puzzleId = e.Value is not null ? (string)e.Value : string.Empty;
        
        if (string.IsNullOrEmpty(puzzleId)) return;
        
        _currentSession = _sessions.FirstOrDefault(s => s.Puzzle.Id == puzzleId)!;
    }

    private void Solve() {
        _currentSession.Solve();
        _activeDialog = "solve";
    }
    
    private async Task Restart() {
        _activeDialog = string.Empty;
        await _currentSession.ClearSession();
    }
}