using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MoveInCheckTests
{

    private TestGameFactory _testGameFactory;

    public MoveInCheckTests()
    {
        _testGameFactory = new TestGameFactory();
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator MoveInCheckTests_EnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }


    // White bishop on 
    // Captures
    // Black rook on 
    // Capture is legal
    [UnityTest]
    public IEnumerator MoveInCheck_RestoresBoardStateAfterCaptureSim()
    {
        /*ARRANGE*/
        TestRuntime.IsTesting = true;
        yield return SceneManager.LoadSceneAsync(
            "SampleScene",
            LoadSceneMode.Single);
        yield return null;

        List<IGamePiece> pieces = new();
        List<Color> colors = new();
        List<int> locations = new();

        GameObject rookObj = new("White Rook");
        Rook rook = rookObj.AddComponent<Rook>();
        Color rColor = Color.White;
        const int ROOK_TILE = 1;
        pieces.Add(rook);
        colors.Add(rColor);
        locations.Add(ROOK_TILE);

        GameObject knightObj = new("Black Knight");
        Knight knight = knightObj.AddComponent<Knight>();
        Color kColor = Color.Black;
        const int KNIGHT_TILE = 33;
        pieces.Add(knight);
        colors.Add(kColor);
        locations.Add(KNIGHT_TILE);

        GameManager gm = _testGameFactory.CreateNPieceGame(
            pieces, colors, locations,
            TestGameFactory.WHITE_KING_LOC, 
            TestGameFactory.BLACK_KING_LOC);

        Game game = gm.Game;
        BoardPlane board = game.Board;

        rook.SetPossibleMoves();
        knight.SetPossibleMoves();

        // Act
        bool moveInCheck = game.MoveSim.MoveInCheck(
            rook,
            game.Board.GetBoardSquare(KNIGHT_TILE),
            game);

        // Assert
        Assert.IsTrue(
            ROOK_TILE == rook.GetCurrentTile().SquareNumber);

        Assert.AreSame(
            rook,
            game.Board.GetOccupier(ROOK_TILE));

        Assert.AreSame(
            knight,
            game.Board.GetOccupier(KNIGHT_TILE));

        Assert.Contains(
            knight.GetObj(),
            game.OtherPlayer.Units);
    }


    /*
     * Second test
    Verify that a pinned piece cannot move.
    Position:
    White Rook
        |
    Black Knight
        |
    Black King
    Moving the bishop should expose the king.
    */
    [UnityTest]
    public IEnumerator MoveInCheck_ReturnsTrue_WhenPinnedPieceMoves()
    {
        /*ARRANGE*/
        TestRuntime.IsTesting = true;
        yield return SceneManager.LoadSceneAsync(
            "SampleScene",
            LoadSceneMode.Single);
        yield return null;

        List<IGamePiece> pieces = new();
        List<Color> colors = new();
        List<int> locations = new();

        GameObject rookObj = new("White Rook");
        Rook rook = rookObj.AddComponent<Rook>();
        Color rColor = Color.White;
        const int ROOK_TILE = 4;
        pieces.Add(rook);
        colors.Add(rColor);
        locations.Add(ROOK_TILE);

        GameObject knightObj = new("Black Knight");
        Knight knight = knightObj.AddComponent<Knight>();
        Color kColor = Color.Black;
        const int KNIGHT_TILE = 52;
        pieces.Add(knight);
        colors.Add(kColor);
        locations.Add(KNIGHT_TILE);

        GameManager gm = _testGameFactory.CreateNPieceGame(
            pieces, colors, locations,
            TestGameFactory.WHITE_KING_LOC + 1,
            TestGameFactory.BLACK_KING_LOC);

        Game game = gm.Game;
        BoardPlane board = game.Board;

        rook.SetPossibleMoves();
        knight.SetPossibleMoves();

        // Act
        bool moveInCheck = game.MoveSim.MoveInCheck(
            knight,
            board.GetBoardSquare(35),
            game);

        // Assert
        Assert.IsTrue(
            ROOK_TILE == rook.GetCurrentTile().SquareNumber);
        Assert.IsTrue(
            KNIGHT_TILE == knight.GetCurrentTile().SquareNumber);

        Assert.AreSame(
            rook,
            game.Board.GetOccupier(ROOK_TILE));
        Assert.AreSame(
            knight,
            game.Board.GetOccupier(KNIGHT_TILE));

        Assert.IsTrue(moveInCheck);

        Assert.Contains(
            rook.GetObj(), 
            game.CurrentPlayer.Units);
        Assert.Contains(
            knight.GetObj(),
            game.OtherPlayer.Units);

        Assert.IsFalse(game.OtherPlayer.UnitsCheckingEnemyKing.Contains(knight));

        Assert.IsFalse(game.OtherPlayer.Check);
    }


    /*
     * Assert unit lists are consistent, unitsCheckingEnemyKing lists are consistent, 
     */

}
