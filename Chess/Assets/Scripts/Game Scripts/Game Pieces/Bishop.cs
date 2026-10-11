public class Bishop : SlidingPiece
{
    private static readonly int[] _directions =
    {
        Movement.NEXT_NW, Movement.NEXT_NE, Movement.NEXT_SW, Movement.NEXT_SE
    };

    protected override int[] Directions => _directions;
}
