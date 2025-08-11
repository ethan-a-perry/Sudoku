using Sudoku.DataAccess.Enums;

namespace Sudoku.DataAccess.Models;

public class GridModel
{
	public int Size { get; set; } = 9;
	public CellModel?[,] Cells { get; set; }
	public GridMode Mode { get; set; } = GridMode.Regular;
	
	/// <summary>
	/// Custom LinkedHashSet that preserves insertion order.
	/// </summary>
	public LinkedHashSet<CellModel>SelectedCells { get; set; } = [];

	public GridModel()
	{
		Cells = new CellModel[Size, Size];
		
		// Initialize the grid by creating an empty CellModel for each position.
		for (int row = 0; row < Size; row++) {
			for (int col = 0; col < Size; col++) {
				Cells[row, col] = new CellModel {
					Row = row,
					Col = col
				};
			}
		}
	}
	
	private void SelectCell(CellModel cell) {
		SelectedCells.Add(cell);
		cell.IsSelected = true;
	}
	
	private void DeselectCell(CellModel cell) {
		SelectedCells.Remove(cell);
		cell.IsSelected = false;
	}

	private void DeselectAllCells() {
		foreach (var cell in SelectedCells.ToList()) {
			DeselectCell(cell);
		}
	}

	// See GridMode.cs for an explanation on the various grid modes/states.
	public void SortSelection(CellModel cell) {
		switch (Mode) {
			case GridMode.Regular:
				if (cell.IsSelected && SelectedCells.Count == 1) {
					DeselectCell(cell);
				}
				else {
					DeselectAllCells();
					SelectCell(cell);
				}
				break;
			case GridMode.Select:
				if (!cell.IsSelected) {
					SelectCell(cell);
				}
				break;
			case GridMode.Delete:
				if (cell.IsSelected) {
					DeselectCell(cell);
				}
				break;
		}
	}

	public void SetGrid(string[,] grid) {
		for (int row = 0; row < grid.GetLength(0); row++) {
			for (int col = 0; col < grid.GetLength(1); col++) {
				// s is your string of length 1 with a digit '1' to '9'
				if (grid[row, col] is [>= '1' and <= '9'] s) {
					Cells[row, col].Value = s[0];
					Cells[row, col].IsGiven = true;
				}
				// If no digit is given, set the value to null
				else {
					Cells[row, col].Value = '\0';
				}
			}
		}
	}

	/// <summary>
	/// Place a character in the cell only if it doesn't alrady contain a given cell.
	/// </summary>
	public void SetCell(char value) {
		foreach (var cell in SelectedCells) {
			if (!cell.IsGiven) {
				cell.Value = value;
			}
		}
	}
}