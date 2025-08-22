namespace Sudoku.Core.Models;

public class PencilMarks
{
    public SortedSet<char> Corner { get; set; } = [];
    public SortedSet<char> Center { get; set; } = [];
}