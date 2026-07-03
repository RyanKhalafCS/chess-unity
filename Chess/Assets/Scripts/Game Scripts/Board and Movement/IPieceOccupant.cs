/// <summary>
/// Minimal interface for objects that can occupy board squares.
/// This allows the Board assembly to be independent of GamePieces.
/// Implementations (like GamePiece) can implement both IGamePiece and IGamePiece.
/// </summary>
public interface IPieceOccupant
{
    void SetCurrentTile(BoardSquare tile);
    BoardSquare GetCurrentTile();

    Color GetColor();
}
