using System.Data;
using Sudoku.DataAccess.Enums;
using Sudoku.DataAccess.Models;

namespace Sudoku.DataAccess.Helpers;

public static class DirectionHelper
{
    /// <summary>
    /// Helps discover neighbors of a given cell by providing the offset coordinates to a potential neighbor cell.
    /// </summary>
    /// <param name="direction">The direction from which a neighbor shall be searched in relation to a given cell</param>
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
    
    /// <summary>
    /// Provides access to the appropriate borders of two orthogonally neighboring cells.
    /// </summary>
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
    
    /// <summary>
    /// Finds the anchor cell of every 2x2 region that contains the specified cell.
    /// </summary>
    /// <remarks>
    /// A 2x2 region consists of four cells arranged in a square that includes the given cell.
    /// Each cell in the grid can belong to up to four such regions.
    /// A region is discovered by locating the given cell's diagonal neighbor(s).
    /// For each discovered 2x2 region, an anchor cell is defined to represent the region.
    /// The anchor cell is always chosen relative to the bottom-right cell of the region.
    /// </remarks>
    /// <param name="cell">The cell from which to find all available 2x2 regions.</param>
    public static IEnumerable<CellModel> GetRegionAnchors(this CellModel cell) {
        // 
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