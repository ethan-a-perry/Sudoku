using Sudoku.DataAccess.Enums;
using Sudoku.DataAccess.Helpers;

namespace Sudoku.DataAccess.Models;

public class CellModel(int row, int col)
{
	public bool IsGiven { get; set; }
	public int Row { get; set; } = row;
	public int Col { get; set; } = col;
	public char? Value { get; set; }
	public bool IsSelected { get; set; }
	
	public SortedSet<char> CornerPencilMarks { get; set; } = new(new PencilMarkComparer());
	public SortedSet<char> CenterPencilMarks { get; set; } = new(new PencilMarkComparer());

	/// <summary>
	/// Limits the number of both corner and center pencil marks to  max of 9.
	/// </summary>
	public bool AddPencilMark(SortedSet<char> set, char value) {
		return (set.Add(value) && set.Count <= 9) || set.Remove(set.Max);
	}
}