using Blazored.LocalStorage;
using Sudoku.Blazor.Client.Services;
using Sudoku.Core.Models;
using Sudoku.DataAccess.Data;
using Sudoku.DataAccess.DataAccess;

namespace Sudoku.Blazor;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder) {
        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();
        
        // MongoDB Access
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<IDbConnection, DbConnection>();
        builder.Services.AddSingleton<IPuzzleData, MongoPuzzleData>();
        
        builder.Services.AddBlazoredLocalStorage();
        
        builder.Services.AddScoped<PuzzleSessionFactory>();
    }
}