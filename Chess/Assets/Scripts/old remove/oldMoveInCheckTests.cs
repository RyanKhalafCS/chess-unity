////using Assets.Scripts.Testing;
////using NUnit.Framework;
////using NUnit.Framework.Internal;
////using Unity.VisualScripting;
////using UnityEngine;
////using static UnityEngine.InputSystem.LowLevel.InputStateHistory;


/////*
////     MoveInCheck() is currently doing two things:

////    Simulating a move.
////    Determining if the move causes check.

////    Because of that, I'd write tests for both:

////    returns correct result
////    restores board state
//// */
////public class MoveInCheckTests
////{
////    private TestGameFactory testGameFactory;

////    public MoveInCheckTests()
////    {
////        this.testGameFactory = new TestGameFactory();
////    }



////    //[Fact]
////    public void MoveInCheck_RestoresBoardState_AfterCaptureSimulation()
////    {
////        // Arrange
////        // White bishop on C1
////        // Captures
////        // Black rook on H6
////        // Capture is legal

////        Game game = testGameFactory.CreateCaptureScenario();

////        IGamePiece bishop = game.CurrentPlayer.Units[0].GetComponent<IGamePiece>();// = GetWhiteBishop(game);


////        var originalTile = bishop.GetCurrentTile();

////        var capturedPiece =
////            game.Board.GetOccupier(48);

////        // Act
////        game.MoveSim.MoveInCheck(
////            bishop,
////            game.Board.GetBoardSquare(48),
////            game);

////        // Assert

////        Assert.Equal(
////            originalTile,
////            bishop.GetCurrentTile());

////        Assert.Same(
////            bishop,
////            game.Board.GetOccupier(
////                originalTile.SquareNumber));

////        Assert.Same(
////            capturedPiece,
////            game.Board.GetOccupier(48));

////        Assert.Contains(
////            capturedPiece.GetObj(),
////            game.OtherPlayer.Units);
////    }
////    /*
////     * [Fact]
////    public void MoveInCheck_RestoresBoardStateAfterSimulation()
////    {
////        // Arrange
////        Game game = CreateGame();

////        IGamePiece whitePiece = GetWhitePiece(game);
////        BoardSquare originalTile = whitePiece.GetCurrentTile();

////        BoardSquare destination =
////            game.Board.GetBoardSquare(originalTile.SquareNumber + 8);

////        IGamePiece originalOccupier =
////            game.Board.GetOccupier(destination.SquareNumber);

////        // Act
////        game.Rules.MoveInCheck(
////            whitePiece,
////            destination,
////            game);

////        // Assert

////        // Piece should be back where it started
////        Assert.Equal(
////            originalTile,
////            whitePiece.GetCurrentTile());

////        // Original square should contain piece
////        Assert.Same(
////            whitePiece,
////            game.Board.GetOccupier(
////                originalTile.SquareNumber));

////        // Destination should be restored
////        Assert.Same(
////            originalOccupier,
////            game.Board.GetOccupier(
////                destination.SquareNumber));
////    }
////     */


///*
// * Second test
//Verify that a pinned piece cannot move.
//Position:
//Black Rook
//    |
//White Bishop
//    |
//White King
//Moving the bishop should expose the king.
//*/

//[Fact]
//public void MoveInCheck_ReturnsTrue_WhenPinnedPieceMoves()
//{
//    // Arrange
//    var game = TestGameFactory.CreatePinnedPieceScenario();

//    var bishop = GetPinnedBishop(game);

//    var destination =
//        game.Board.GetBoardSquare(35);

//    // Act
//    bool result =
//        game.MoveSim.MoveInCheck(
//            bishop,
//            destination,
//            game);

//    // Assert
//    Assert.True(result);
//}


//    /*
//     [Fact]
//public void MoveInCheck_ReturnsTrue_WhenMoveExposesKing()
//{
//    // Arrange
//    Game game = CreatePinnedPieceScenario();

//    IGamePiece pinnedPiece = GetPinnedPiece(game);

//    BoardSquare illegalMove =
//        game.Board.GetBoardSquare( square );

//    // Act
//    bool result =
//        game.Rules.MoveInCheck(
//            pinnedPiece,
//            illegalMove,
//            game);

//    // Assert
//    Assert.True(result);
//}
//*/

//    /*
//     * [Fact]
//public void MoveInCheck_RestoresCapturedPieceAfterSimulation()
//{
//    // Arrange

//    Game game = CreateCaptureScenario();

//    IGamePiece attacker = GetWhiteAttacker(game);
//    IGamePiece defender = GetBlackTarget(game);

//    BoardSquare defenderSquare =
//        defender.GetCurrentTile();

//    // Act

//    game.Rules.MoveInCheck(
//        attacker,
//        defenderSquare,
//        game);

//    // Assert

//    Assert.Same(
//        defender,
//        game.Board.GetOccupier(
//            defenderSquare.SquareNumber));

//    Assert.Equal(
//        defenderSquare,
//        defender.GetCurrentTile());
//}
//     */
//}
