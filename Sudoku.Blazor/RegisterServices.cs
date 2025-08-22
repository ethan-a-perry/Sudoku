using Sudoku.Blazor.Services;
using Sudoku.Core.Models;
using Sudoku.DataAccess.Data;
using Sudoku.DataAccess.DataAccess;

namespace Sudoku.Blazor;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder) {
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        
        builder.Services.AddMemoryCache();

        builder.Services.AddSingleton<IDbConnection, DbConnection>();
        builder.Services.AddSingleton<IPuzzleData, MongoPuzzleData>();

        builder.Services.AddScoped<Grid>(provider => new Grid(9, 9));
        builder.Services.AddScoped<SelectionManager>();
        builder.Services.AddScoped<InputManager>();
    }
}