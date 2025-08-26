using Microsoft.AspNetCore.Components;
using Sudoku.Blazor.Client.Services;
using Sudoku.DataAccess.Data;
using Sudoku.DataAccess.Services;
using Sudoku.DataAccess.Models;

namespace Sudoku.Blazor.Client.Components.Shared;

public partial class SudokuGrid : ComponentBase
{
    [Inject] private IPuzzleData PuzzleData { get; set; }
    private List<PuzzleModel> _puzzles = [];
    
    private List<PuzzleSession> _sessions = [];
    private PuzzleSession _currentSession;
    
    private InputManager _inputManager;
    private SelectionManager _selectionManager;
    private UndoRedoService _undoRedoService;
    
    private SudokuSolver SudokuSolver { get; set; } = new();
    
    protected override async Task OnInitializedAsync() {
        _puzzles = await PuzzleData.GetAllPuzzles();

        foreach (var puzzle in _puzzles) {
            _sessions.Add(new PuzzleSession(puzzle));
        }
        
        _currentSession = _sessions[0];
        
        _selectionManager = new SelectionManager(_currentSession);
        _undoRedoService = new UndoRedoService(_currentSession, _selectionManager);
        _inputManager = new InputManager(_currentSession, _selectionManager, _undoRedoService);
    }

    private void SwitchSession(PuzzleModel puzzle) {
        _currentSession = _sessions.FirstOrDefault(s => s.Id == puzzle.Id)!;
        
        _selectionManager.SetCurrentSession(_currentSession);
        _undoRedoService.SetCurrentSession(_currentSession);
        _inputManager.SetCurrentSession(_currentSession);
    }
    
    private void Solve() {
        Console.WriteLine(SudokuSolver.IsSolved(_currentSession.Grid) ? "Solved" : "Not solved");
    }
}