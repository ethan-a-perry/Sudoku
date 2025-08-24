using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sudoku.Blazor.Client;
using Sudoku.Blazor.Client.Components.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.ConfigureServices();

builder.RootComponents.Add<SudokuGrid>("#sudoku");

await builder.Build().RunAsync();