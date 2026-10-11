public class Queen : SlidingPiece
{
    private static readonly int[] _directions =
    {
        Movement.NEXT_N, Movement.NEXT_S, Movement.NEXT_E, Movement.NEXT_W,
        Movement.NEXT_NW, Movement.NEXT_NE, Movement.NEXT_SW, Movement.NEXT_SE
    };

    protected override int[] Directions => _directions;
}
