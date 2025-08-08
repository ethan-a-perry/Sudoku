using Sudoku.DataAccess.Enums;
using Sudoku.DataAccess.Helpers;

namespace Sudoku.DataAccess.Models;

public class GridModel
{
	public int Size { get; set; } = 9;
	public CellModel?[,] Cells { get; set; }
	public GridMode Mode { get; set; } = GridMode.Regular;
	public LinkedHashSet<CellModel>SelectedCells { get; set; } = [];

	public GridModel()
	{
		Cells = new CellModel[Size, Size];
		
		for (int row = 0; row < Size; row++) {
			for (int col = 0; col < Size; col++) {
				Cells[row, col] = new CellModel {
					Row = row,
					Col = col
				};
			}
		}

		foreach (var cell in Cells) {
			foreach (var direction in Enum.GetValues<Direction>()) {
				var offset = DirectionHelper.Offset(direction);
				
				(int row, int col) neighbor = (cell.Row + offset.row, cell.Col + offset.col);
				
				if (neighbor.row >= 0 && neighbor.row < Size && neighbor.col >= 0 && neighbor.col < Size) {
					cell.Neighbors[direction] = Cells[neighbor.row, neighbor.col];
				}
			}
		}
	}
	
	private void AdjustCorners(CellModel cell) {
		foreach (var anchor in cell.GetRegionAnchors()) {
			var topLeft = anchor.Neighbors[Direction.TopLeft];
			var topRight = anchor.Neighbors[Direction.Top];
			var bottomLeft = anchor.Neighbors[Direction.Left];
			// bottomRight = anchor

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
				default:
					topLeft.Borders &= ~Borders.BottomRightCorner;
					topRight.Borders &= ~Borders.BottomLeftCorner;
					bottomLeft.Borders &= ~Borders.TopRightCorner;
					anchor.Borders &= ~Borders.TopLeftCorner;
					break;
			}
		}
	}

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

	public void SetCell(char value) {
		foreach (var cell in SelectedCells) {
			if (!cell.IsGiven) {
				cell.Value = value;
			}
		}
	}

	public void TraverseGrid(string arrowKey) {
		var lastSelectedCell = SelectedCells.GetLastSelected();
		
		(Direction direction, (int row, int col) fallback) =
			arrowKey switch {
				"ArrowUp" => (Direction.Top, (Size - 1, lastSelectedCell.Col)),
				"ArrowRight" => (Direction.Right, (lastSelectedCell.Row, 0)),
				"ArrowDown" => (Direction.Bottom, (0, lastSelectedCell.Col)),
				"ArrowLeft" => (Direction.Left, (lastSelectedCell.Row, Size - 1))
			};

		SortSelection(lastSelectedCell.Neighbors.TryGetValue(direction, out var neighbor)
			? neighbor
			: Cells[fallback.row, fallback.col]
		);
	}
}