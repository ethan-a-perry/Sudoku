using System.Data;
using Sudoku.DataAccess.Enums;
using Sudoku.DataAccess.Helpers;

namespace Sudoku.DataAccess.Models;

public class GridModel
{
	public int Size { get; set; } = 9;
	public CellModel?[,] Cells { get; set; }
	public GridMode Mode { get; set; } = GridMode.Regular;

	public InputMode InputMode { get; set; } = InputMode.Digit;
	
	public LinkedHashSet<CellModel>SelectedCells { get; set; } = [];

	public List<List<(int Row, int Col, char? Value)>> Moves { get; set; } = [];

	public GridModel()
	{
		Cells = new CellModel[Size, Size];
		
		// Initialize the grid by creating an empty CellModel for each position.
		for (int row = 0; row < Size; row++) {
			for (int col = 0; col < Size; col++) {
				Cells[row, col] = new CellModel(row, col);
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
					Cells[row, col].IsGiven = false;
				}
			}
		}
	}

	/// <summary>
	/// Removes digits, then corner penicl marks, then center pencil
	/// marks in that order unless overridden by the InputMode
	/// </summary>
	public void UnsetCellValue(char delete) {
		foreach (var cell in SelectedCells) {
			if (cell.IsGiven) continue;

			if (cell.Value is not '\0') {
				cell.Value = delete;
				continue;
			}
			
			if (cell.CornerPencilMarks.Count > 0 && (InputMode is not InputMode.CenterPencilMark || cell.CenterPencilMarks.Count == 0)) { 
				cell.CornerPencilMarks.Clear();
				continue;
			}

			cell.CenterPencilMarks.Clear();
		}
	}
	
	public void SetCellValue(char value) {
		foreach (var cell in SelectedCells) {
			if (cell.IsGiven) continue;

			if (cell.Value == value) {
				cell.Value = '\0';
				continue;
			}
			
			switch (InputMode) {
				case InputMode.Digit:
					cell.Value = value;
					break;
				case InputMode.CornerPencilMark:
					if (!cell.AddPencilMark(cell.CornerPencilMarks, value)) {
						cell.CornerPencilMarks.Remove(value);
					}
					break;
				case InputMode.CenterPencilMark:
					if (!cell.AddPencilMark(cell.CenterPencilMarks, value)) {
						cell.CenterPencilMarks.Remove(value);
					}
					break;
			}
		}
	}
	
	public void TakeSnapshot() {
		// Convert SelectedCells into a list of tuples, preserving insertion order
		var snapshot = SelectedCells
			.Select(cell => (cell.Row, cell.Col, cell.Value))
			.ToList();

		Moves.Add(snapshot);

		// Keep only the latest 16 snapshots
		if (Moves.Count > 16) {
			Moves.RemoveAt(0);
		}
	}
	
	public void RestoreSnapshot() {
		if (Moves.Count == 0)
			return;
		
		var snapshot = Moves[^1];
		Moves.RemoveAt(Moves.Count - 1);
		
		DeselectAllCells();
		foreach (var cell in snapshot) {
			SelectCell(Cells[cell.Row, cell.Col]);
			Cells[cell.Row, cell.Col].Value = cell.Value;
		}
	}
}