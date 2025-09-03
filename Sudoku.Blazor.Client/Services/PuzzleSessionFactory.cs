using Blazored.LocalStorage;
using Sudoku.DataAccess.Models;

namespace Sudoku.Blazor.Client.Services;

public class PuzzleSessionFactory(ILocalStorageService localStorage)
{
    public async Task<PuzzleSession> Create(PuzzleModel puzzle) {
        var session = new PuzzleSession(localStorage);
        await session.Initialize(puzzle);
        return session;
    }
}