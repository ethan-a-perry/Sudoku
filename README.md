# Sudoku App

An interactive Sudoku app featuring playable puzzles and full pencil mark support.

It is built in C# with the Blazor ecosystem and deployed on my personal website using Astro.
This project was an exercise in building a full-featured, interactive Sudoku app in C# without using any JavaScript.

Visit my personal website to play the puzzle and learn more about this project.

[Play here](https://ethanperry.net/work/sudoku)

**Current features:**
* Playable puzzles with validation
* Full pencil mark notation support (center and corner pencil marks)
* Undo/Redo functionality
* Clean, responsive design
* Light/dark theme

**Technologies:**
* C#
* .NET 9
* Blazor Web App (Server & WebAssembly)
* MongoDB
* HTML
* CSS
* Session storage and local storage
* Deployed on my personal website using Astro.

## Architecture & Deployment Notes

This app is built in .NET 9 and is structured as a Blazor web app with both server-side and WebAssembly (WASM) rendering
support.

The client side (WASM) handles all interactive gameplay and logic, while the server side renders components on
the frontend. When rendered through the server, puzzle data is loaded from MongoDB, but in WebAssembly mode, puzzle
data is loaded from a local JSON file embedded within the project for fast, offline-friendly access.

Rather than building a standalone Blazor WebAssembly app, keeping the server-client structure allows integration with
MongoDB and demonstrates different data access methods depending on the rendering mode.

Lastly, rather than deploying the app as a standalone app, it is published and embedded into my Astro-based personal
website, which manages global layout and styling and offers a more unified user experience.

## Installation
Prerequisites:
* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

Clone the repository:
```shell
git clone https://github.com/perry-ethan/Sudoku.git
```

To run the web app locally, you must execute it from the server-side project (Sudoku.Blazor) as this project is not a standalone WebAssembly app.
Navigate to the correct directory and run the server:
```shell
cd Sudoku/Sudoku.Blazor
dotnet run
```

Navigate to the specified address to use the app.

If you would like to publish the client as an independent WebAssembly app to take or use elsewhere, you can do so with the following instructions.
This code will place the WASM project files in the /publish directory (or somewhere else if you specify different) in the project root.

```shell
cd Sudoku
dotnet publish Sudoku.Blazor.Client -c Release -o ./publish
```

Note: For use in my Astro website, I only needed the `/_framework` and `/sudoku/data` directory, both of which were
located in the `/publish/wwwroot` directory.

## Acknowledgements
This project is my own implementation of a full-featured Sudoku app, built entirely in C# with Blazor, without using any JavaScript.

I want to emphasize that this project is entirely my own implementation, though it was inspired by my experience
playing Sudoku on [Sven's Sudoku Pad](https://sudokupad.app/)(featured on [Cracking the Cryptic YouTube channel](https://www.youtube.com/@CrackingTheCryptic)) and,
more loosely, on the [New York Times Sudoku](https://www.nytimes.com/puzzles/sudoku) website.