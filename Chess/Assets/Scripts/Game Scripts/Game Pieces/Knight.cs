using System.Collections.Generic;

public class Knight : GamePiece
{
    //NW = NORTH WEST, so knight goes north 2 then west 1.
    public static readonly int NW = 17;
    public static readonly int NE = 15;
    public static readonly int SW = -15;
    public static readonly int SE = -17;
    public static readonly int EN = 6;
    public static readonly int ES = -10;
    public static readonly int WN = 10;
    public static readonly int WS = -6;

    private static readonly int[] _offsets = { NW, NE, WN, WS, SW, SE, EN, ES };

    protected override void GenerateMoves(List<BoardSquare> moves, List<BoardSquare> attacks)
    {
        foreach (int dir in _offsets)
        {
            int squareNum = CurrentTile.SquareNumber + dir;
            if (BoardPlane.InBounds(squareNum) && !NearEdge(squareNum - dir, dir))
            {
                TryAddSquare(moves, attacks, squareNum);
            }
        }
    }

    public static bool NearEdge(int squareNum, int dir)
    {
        int index = squareNum - 1;
        int row = index / 8;
        int col = index % 8;
        if (dir == NW && (row >= 6 || col == 7))
            return true;
        if (dir == NE && (row >= 6 || col == 0))
            return true;
        if (dir == WN && (row == 7 || col >= 6))
            return true;
        if (dir == WS && (row == 0 || col >= 6))
            return true;
        if (dir == EN && (row == 7 || col <= 1))
            return true;
        if (dir == ES && (row == 0 || col <= 1))
            return true;
        if (dir == SW && (row <= 1 || col == 7))
            return true;
        if (dir == SE && (row <= 1 || col == 0))
            return true;
        return false;
    }
}
