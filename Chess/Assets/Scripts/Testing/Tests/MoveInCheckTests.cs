using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MoveInCheckTests
{

    private TestGameFactory testGameFactory;

    public MoveInCheckTests()
    {
        this.testGameFactory = new TestGameFactory();
    }
    // A Test behaves as an ordinary method
    [Test]
    public void MoveInCheckTests_SimplePasses()
    {
        // Use the Assert class to test conditions
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
    public IEnumerator MoveInCheck_RestoresBoardState_AfterCaptureSimulation()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        // Arrange
        Game game = testGameFactory.CreateCaptureScenario(
            TestGameFactory.WHITE_KING_LOC, TestGameFactory.BLACK_KING_LOC);
        BoardPlane board = game.Board;


        IGamePiece bishop = board.GetOccupier(19);//game.CurrentPlayer.Units[0].GetComponent<IGamePiece>();// RETHINK THE FOLLOWING BIT
        var originalTile = bishop.GetCurrentTile();
        IGamePiece capturedPiece = game.Board.GetOccupier(46);//rook
        IGamePiece king = game.Board.GetOccupier(60);

        // Verify correct setup
        Assert.AreSame(bishop, board.GetOccupier(19));
        Assert.AreSame(capturedPiece, board.GetOccupier(46));
        Assert.AreSame(originalTile, bishop.GetCurrentTile());
        Assert.AreSame(originalTile, board.GetOccupier(19).GetCurrentTile());
        Assert.AreSame(king, board.GetOccupier(60));

        // Act
        bool moveInCheck = game.MoveSim.MoveInCheck(//make sure everything looks good. odd results in IsInCheck() call
            bishop,
            game.Board.GetBoardSquare(46),
            game);

        // Assert
        Assert.AreEqual(moveInCheck, false);
        Assert.AreEqual(
            originalTile,
            bishop.GetCurrentTile());

        Assert.AreSame(
            bishop,
            game.Board.GetOccupier(
                originalTile.SquareNumber));

        Assert.AreSame(
            capturedPiece,
            game.Board.GetOccupier(46));

        Assert.Contains(
            capturedPiece.GetObj(),
            game.OtherPlayer.Units);
    }


    /*
     * Second test
    Verify that a pinned piece cannot move.
    Position:
    Black Rook
        |
    White Bishop
        |
    White King
    Moving the bishop should expose the king.
    */
    [UnityTest]
    public IEnumerator MoveInCheck_ReturnsTrue_WhenPinnedPieceMoves()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        // Arrange
        var game = testGameFactory.CreatePinnedPieceScenario();

        IGamePiece bishop = null;// = GetPinnedBishop(game);

        var destination =
            game.Board.GetBoardSquare(35);

        // Act
        bool result =
            game.MoveSim.MoveInCheck(
                bishop,
                destination,
                game);

        // Assert
        Assert.True(result);
    }

}
