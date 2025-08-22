using Sudoku.Core.Records;

namespace Sudoku.Core.Models;

public class Snapshot(List<CellState> before, List<CellState> after)
{
    public List<CellState> Before { get; set; } = before;
    public List<CellState> After { get; set; } = after;

    public Snapshot() : this([], []) { }
}