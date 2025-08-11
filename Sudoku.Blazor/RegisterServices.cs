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
    }
}