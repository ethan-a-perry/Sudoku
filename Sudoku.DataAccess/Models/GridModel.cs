using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sudoku.DataAccess.Enums;

namespace Sudoku.DataAccess.Models;

public class GridModel
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string Id { get; set; }

	public int Size { get; set; } = 9;
	public CellModel?[,] Cells { get; set; }
	
	public LinkedHashSet<CellModel> SelectedCells { get; set; } = [];
	
	public GridMode Mode { get; set; } = GridMode.Regular;

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
			if (cell.Row > 0) {
				cell.Neighbors[Direction.Up] = Cells[cell.Row - 1, cell.Col];
			}

			if (cell.Col < Size - 1) {
				cell.Neighbors[Direction.Right] = Cells[cell.Row, cell.Col + 1];
			}
			
			if (cell.Row < Size - 1) {
				cell.Neighbors[Direction.Down] = Cells[cell.Row + 1, cell.Col];
			}

			if (cell.Col > 0) {
				cell.Neighbors[Direction.Left] = Cells[cell.Row, cell.Col - 1];
			}
		}
	}

	private void SelectCell(CellModel cell) {
		SelectedCells.Add(cell);
		cell.IsSelected = true;
		UpdateBorders(cell);
	}

	private void DeselectCell(CellModel cell) {
		SelectedCells.Remove(cell);
		cell.IsSelected = false;
		UpdateBorders(cell);
	}

	public void DeselectAllCells() {
		foreach (var cell in SelectedCells.ToList()) {
			DeselectCell(cell);
		}
	}

	private void UpdateBorders(CellModel cell) {
		cell.Borders = cell.IsSelected ? Borders.All : Borders.None;

		Dictionary<Direction, (Borders CellBorder, Borders NeighborBorder)> borderMap = new() {
			[Direction.Up] = (Borders.Top, Borders.Bottom),
			[Direction.Right] = (Borders.Right, Borders.Left),
			[Direction.Down] = (Borders.Bottom, Borders.Top),
			[Direction.Left] = (Borders.Left, Borders.Right)
		};

		foreach (var (direction, neighbor) in cell.Neighbors) {
			if (!neighbor.IsSelected) continue;
			
			var (cellBorder, neighborBorder) = borderMap[direction];
			
			if (cell.IsSelected) {
				cell.Borders &= ~cellBorder;
				neighbor.Borders &= ~neighborBorder;
			}
			else {
				neighbor.Borders |= neighborBorder;
			}
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
}