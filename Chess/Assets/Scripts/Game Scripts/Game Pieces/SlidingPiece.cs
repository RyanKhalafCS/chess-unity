using System.Collections.Generic;

/// <summary>
/// Base for pieces that slide any number of squares along fixed directions
/// (Bishop, Rook, Queen). Subclasses only supply the directions.
/// </summary>
public abstract class SlidingPiece : GamePiece
{
    protected abstract int[] Directions { get; }

    protected override void GenerateMoves(List<BoardSquare> moves, List<BoardSquare> attacks)
    {
        foreach (int dir in Directions)
        {
            Slide(moves, attacks, dir);
        }
    }

    private void Slide(List<BoardSquare> moves, List<BoardSquare> attacks, int dir)
    {
        int squareNum = CurrentTile.SquareNumber + dir;
        while (BoardPlane.InBounds(squareNum) && !Movement.IsOverEdge(squareNum - dir, dir))
        {
            if (!TryAddSquare(moves, attacks, squareNum))
                break;
            squareNum += dir;
        }
    }
}
