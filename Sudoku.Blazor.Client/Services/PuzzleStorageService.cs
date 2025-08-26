using Blazored.LocalStorage;
using Sudoku.Core.Models;

namespace Sudoku.Blazor.Client.Services;

public class PuzzleStorageService(ILocalStorageService localStorage)
{
    public async Task SaveGrid(string id, Grid grid) {
        await localStorage.SetItemAsync($"puzzle_{id}", grid);
    }
    
    public async Task<Grid?> LoadGrid(string id) {
        return await localStorage.GetItemAsync<Grid>($"puzzle_{id}");
    }
}