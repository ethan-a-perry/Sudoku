using Sudoku.DataAccess.Enums;
using Sudoku.DataAccess.Helpers;

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

		// Establish neighbor relationships for every cell in the grid.
		foreach (var cell in Cells) {
			foreach (var direction in Enum.GetValues<Direction>()) {
				var offset = DirectionHelper.Offset(direction);
				
				(int row, int col) neighbor = (cell.Row + offset.row, cell.Col + offset.col);
				
				/*
				 * If the calculated neighbor coordinates fall outside the grid boundaries,
				 * the cell is on an edge in that direction and has no neighbor there.
				 * Otherwise, assign the neighboring cell accordingly.
				*/
				if (neighbor.row >= 0 && neighbor.row < Size && neighbor.col >= 0 && neighbor.col < Size) {
					cell.Neighbors[direction] = Cells[neighbor.row, neighbor.col];
				}
			}
		}
	}
	
	/// <summary>
	/// Detects edge turns within 2x2 regions containing the given cell and applies corner borders
	/// to maintain a continuous visual outline without breaks.
	/// </summary>
	/// <param name="cell">The cell for which to adjust corner borders based on its regions.</param>
	private void AdjustCorners(CellModel cell) {
		foreach (var anchor in cell.GetRegionAnchors()) {
			var topLeft = anchor.Neighbors[Direction.TopLeft];
			var topRight = anchor.Neighbors[Direction.Top];
			var bottomLeft = anchor.Neighbors[Direction.Left];
			// bottomRight as denoted by "anchor".

			// There are exactly 4 patterns where three cells are selected in a 2x2 region,
			// thus indicating an edge-turn is present which requires a corner border.
			switch (topLeft.IsSelected, topRight.IsSelected, bottomLeft.IsSelected, anchor.IsSelected) {
				case (false, true, true, true):
					anchor.Borders |= Borders.TopLeftCorner;
					break;
				case (true, false, true ,true):
					bottomLeft.Borders |= Borders.TopRightCorner;
					break;
				case (true, true, false, true):
					topRight.Borders |= Borders.BottomLeftCorner;
					break;
				case (true, true, true ,false):
					topLeft.Borders |= Borders.BottomRightCorner;
					break;
				// If no edge-turn exists, remove any potential corner borders applied earlier.
				default:
					topLeft.Borders &= ~Borders.BottomRightCorner;
					topRight.Borders &= ~Borders.BottomLeftCorner;
					bottomLeft.Borders &= ~Borders.TopRightCorner;
					anchor.Borders &= ~Borders.TopLeftCorner;
					break;
			}
		}
	}

	/// <summary>
	/// Adjust border styling when selecting and unselecting any cells.
	/// </summary>
	private void AdjustBorders(CellModel cell) {
		if (cell.IsSelected) {
			cell.Borders |= Borders.AllBorders;
		}
		else {
			cell.Borders &= ~Borders.AllBorders;
		}
		
		foreach (var (direction, neighbor) in cell.GetOrthogonalNeighbors()) {
			if (!neighbor.IsSelected) continue;

			if (cell.IsSelected) {
				var (cellBorder, neighborBorder) = DirectionHelper.GetEdge(direction);
				cell.Borders &= ~cellBorder;
				neighbor.Borders &= ~neighborBorder;
			}
			else {
				neighbor.Borders |= DirectionHelper.GetEdge(direction).neighborBorder;
			}
		}

		AdjustCorners(cell);
	}
	
	private void SelectCell(CellModel cell) {
		SelectedCells.Add(cell);
		cell.IsSelected = true;
		AdjustBorders(cell);
	}
	
	private void DeselectCell(CellModel cell) {
		SelectedCells.Remove(cell);
		cell.IsSelected = false;
		AdjustBorders(cell);
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