using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sudoku.Blazor.Client;
using Sudoku.Blazor.Client.Components.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Mount the SudokuGrid component into the DOM element with id="sudoku-app"
builder.RootComponents.Add<SudokuGrid>("#sudoku-app");

builder.ConfigureServices();

await builder.Build().RunAsync();