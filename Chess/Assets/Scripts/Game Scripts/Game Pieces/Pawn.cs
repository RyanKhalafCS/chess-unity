using System.Collections.Generic;

public class Pawn : GamePiece
{
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
        int colorMultiplier = Movement.GetColorMultiplier(PieceColor);

        //Attack NW and NE
        TryAddAttack(attacks, Movement.NEXT_NW * colorMultiplier);
        TryAddAttack(attacks, Movement.NEXT_NE * colorMultiplier);

        //Move forward
        TryAddForwardMoves(moves, Movement.NEXT_N * colorMultiplier);
    }

    private void TryAddAttack(List<BoardSquare> attacks, int dir)
    {
        int squareNum = CurrentTile.SquareNumber + dir;
        if (BoardPlane.InBounds(squareNum)
            && !Movement.IsOverEdge(squareNum - dir, dir)
            && Board.CanAttack(PieceColor, squareNum))
        {
            attacks.Add(Board.GetBoardSquare(squareNum));
        }
    }

    private void TryAddForwardMoves(List<BoardSquare> moves, int dir)
    {
        int squareNum = CurrentTile.SquareNumber + dir;
        if (!BoardPlane.InBounds(squareNum) || !Board.CanMove(squareNum))
            return;

        moves.Add(Board.GetBoardSquare(squareNum));

        //Second tile forward on the pawn's first move
        squareNum += dir;
        if (BoardPlane.InBounds(squareNum) && FirstMove && Board.CanMove(squareNum))
        {
            moves.Add(Board.GetBoardSquare(squareNum));
        }
    }
}
