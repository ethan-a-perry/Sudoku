using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sudoku.Blazor.Client.Services;
using Sudoku.DataAccess.Data;
using Sudoku.DataAccess.Services;
using Sudoku.DataAccess.Models;

namespace Sudoku.Blazor.Client.Components.Shared;

public partial class SudokuGrid : ComponentBase
{
    [Inject] IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorageService { get; set; }
    [Inject] private IPuzzleData PuzzleData { get; set; }

    private List<PuzzleModel> _puzzles = [];
    
    private List<PuzzleSession> _sessions = [];
    private PuzzleSession? _currentSession;
    
    private SudokuSolver SudokuSolver { get; set; } = new();
    private bool _isSolved;
    
    private bool _isRestartModalVisible;
    private bool _isSolveModalVisible;

    protected override async Task OnInitializedAsync() {
        _puzzles = await PuzzleData.GetAllPuzzles();
        
        foreach (var puzzle in _puzzles) {
            var newSession = new PuzzleSession(LocalStorageService);
            await newSession.InitializeAsync(puzzle);
            _sessions.Add(newSession);
        }
        
        _currentSession = _sessions[0];
        
        await JSRuntime.InvokeVoidAsync("eval", "window.dispatchEvent(new Event('sudokuAppReady'));");
    }
    
    private async Task LoadSession(ChangeEventArgs e) {
        var puzzleId = e.Value is not null ? (string)e.Value : string.Empty;
        
        if (string.IsNullOrEmpty(puzzleId)) return;
        
        _currentSession = _sessions.FirstOrDefault(s => s.Puzzle.Id == puzzleId)!;
    }
    
    private void OpenRestartModal() {
        _isSolveModalVisible = false;
        _isRestartModalVisible = true;
    }
    
    private void OpenSolveModal() {
        _isRestartModalVisible = false;
        _isSolveModalVisible = true;
    }
    
    private void CloseModal() {
        _isRestartModalVisible = false;
        _isSolveModalVisible = false;
    }
    
    private async Task Restart() {
        _isSolved = false;
        CloseModal();
        var currentPuzzle = _puzzles.FirstOrDefault(p => p.Id == _currentSession.Puzzle.Id);
    
        _currentSession = new PuzzleSession(LocalStorageService);
        await _currentSession.InitializeAsync(currentPuzzle);
    }
    
    private void Solve() {
        _isSolved = SudokuSolver.IsSolved(_currentSession.Grid);
        OpenSolveModal();
    }
}