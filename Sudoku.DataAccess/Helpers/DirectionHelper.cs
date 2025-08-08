using System.Data;
using Sudoku.DataAccess.Enums;
using Sudoku.DataAccess.Models;

namespace Sudoku.DataAccess.Helpers;

public static class DirectionHelper
{
    public static (int row, int col) Offset(Direction direction) {
        return direction switch {
            Direction.Top => (-1, 0),
            Direction.Right => (0, 1),
            Direction.Bottom => (1, 0),
            Direction.Left => (0, -1),
            Direction.TopLeft => (-1, -1),
            Direction.TopRight => (-1, 1),
            Direction.BottomLeft => (1, -1),
            Direction.BottomRight => (1, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction")
        };
    }
    
    public static (Borders cellBorder, Borders neighborBorder) GetEdge(Direction direction) {
        return direction switch {
            Direction.Top => (Borders.Top, Borders.Bottom),
            Direction.Right => (Borders.Right, Borders.Left),
            Direction.Bottom => (Borders.Bottom, Borders.Top),
            Direction.Left => (Borders.Left, Borders.Right),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction")
        };
    }

    public static IEnumerable<(Direction, CellModel)> GetDiagonalNeighbors(this CellModel cell) {
        foreach (var (direction, neighbor) in cell.Neighbors) {
            if (direction is Direction.TopLeft or Direction.TopRight or Direction.BottomLeft or Direction.BottomRight) {
                yield return (direction, neighbor);
            }
        }
    }
    
    public static IEnumerable<(Direction, CellModel)> GetOrthogonalNeighbors(this CellModel cell) {
        foreach (var (direction, neighbor) in cell.Neighbors) {
            if (direction is Direction.Top or Direction.Right or Direction.Bottom or Direction.Left) {
                yield return (direction, neighbor);
            }
        }
    }

    public static IEnumerable<CellModel> GetRegionAnchors(this CellModel cell) {
        foreach (var (direction, neighbor) in cell.GetDiagonalNeighbors()) {
            yield return direction switch {
                Direction.TopLeft => cell,
                Direction.TopRight => cell.Neighbors[Direction.Right],
                Direction.BottomLeft => cell.Neighbors[Direction.Bottom],
                Direction.BottomRight => neighbor,
                _ => cell
            };
        }
    }
}