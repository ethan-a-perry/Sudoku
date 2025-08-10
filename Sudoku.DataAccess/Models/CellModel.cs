using Sudoku.DataAccess.Enums;

namespace Sudoku.DataAccess.Models;

public class CellModel
{
	public bool IsGiven { get; set; }
	public int Row { get; set; }
	public int Col { get; set; }
	public char? Value { get; set; }
	public bool IsSelected { get; set; }
}