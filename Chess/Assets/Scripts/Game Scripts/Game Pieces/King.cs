using System.Collections.Generic;

public class King : GamePiece
{
    private static readonly int[] _directions =
    {
        Movement.NEXT_N, Movement.NEXT_S, Movement.NEXT_E, Movement.NEXT_W,
        Movement.NEXT_NE, Movement.NEXT_NW, Movement.NEXT_SE, Movement.NEXT_SW
    };

    public bool FirstMove { get; set; }

    protected override void OnInitialize()
    {
        FirstMove = true;
    }

    public override void OnMoved()
    {
        FirstMove = false;
    }

    protected override void GenerateMoves(List<BoardSquare> moves, List<BoardSquare> attacks)
    {
        foreach (int dir in _directions)
        {
            int squareNum = CurrentTile.SquareNumber + dir;
            if (BoardPlane.InBounds(squareNum) && !Movement.IsOverEdge(squareNum - dir, dir))
            {
                TryAddSquare(moves, attacks, squareNum);
            }
        }
    }
}
