using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sudoku.Blazor.Client.Services;
using Sudoku.Core.Models;
using Sudoku.DataAccess.Data;

namespace Sudoku.Blazor.Client;

public static class RegisterServices
{
    public static void ConfigureServices(this WebAssemblyHostBuilder builder) {
        builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        
        builder.Services.AddSingleton<IPuzzleData, JsonPuzzleData>();
        
        builder.Services.AddScoped<Grid>(provider => new Grid(9, 9));
        builder.Services.AddScoped<UndoRedoService>();
        builder.Services.AddScoped<SelectionManager>();
        builder.Services.AddScoped<InputManager>();
    }
}