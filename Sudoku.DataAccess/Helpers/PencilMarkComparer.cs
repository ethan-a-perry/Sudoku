namespace Sudoku.DataAccess.Helpers;

/// <summary>
/// Sorts in the following order: Digits, Letters, then anything else
/// </summary>
public class PencilMarkComparer : IComparer<char>
{
    public int Compare(char x, char y) {
        int xGroup = GetGroup(x);
        int yGroup = GetGroup(y);
        
        return xGroup != yGroup ? xGroup.CompareTo(yGroup) : x.CompareTo(y);
    }

    private int GetGroup(char c) {
        if (char.IsDigit(c)) return 0;
        if (char.IsLetter(c)) return 1;
        return 2;
    }
}