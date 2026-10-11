public class Rook : SlidingPiece, IPieceOccupant
{
    private static readonly int[] _directions =
    {
        Movement.NEXT_N, Movement.NEXT_S, Movement.NEXT_E, Movement.NEXT_W
    };

    public bool FirstMove { get; set; }

    protected override int[] Directions => _directions;

    protected override void OnInitialize()
    {
        FirstMove = true;
    }

    public override void OnMoved()
    {
        FirstMove = false;
    }
}
