using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class DestroyUnitTests
{

    private TestGameFactory _testGameFactory;

    public DestroyUnitTests()
    {
        _testGameFactory = new TestGameFactory();
    }

    [Test]
    public void DestroyUnitTests_SimplePasses()
    {
        // Use the Assert class to test conditions
    }


    //Queen attacks Pawn
    [UnityTest]
    public IEnumerator DestroyUnitTest_QueenAttackPawn()
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

        GameObject queenObj = new("White Queen");
        Queen queen = queenObj.AddComponent<Queen>();
        Color qColor = Color.White;
        const int QUEEN_TILE = 5;
        pieces.Add(queen);
        colors.Add(qColor);
        locations.Add(QUEEN_TILE);

        GameObject pawnObj = new("Black Pawn");
        Pawn pawn = pawnObj.AddComponent<Pawn>();
        Color pColor = Color.Black;
        const int PAWN_TILE = 33;
        pieces.Add(pawn);
        colors.Add(pColor);
        locations.Add(PAWN_TILE);

        GameManager gm = _testGameFactory.CreateNPieceGame(
            pieces, colors, locations,
            TestGameFactory.WHITE_KING_LOC,
            TestGameFactory.BLACK_KING_LOC);

        Game game = gm.Game;
        BoardPlane board = game.Board;

        /*ACT*/
        gm.RequestMove(queenObj, board.GetBoardSquare(PAWN_TILE));
        yield return null;

        /*ASSERT*/
        Assert.IsTrue(pawnObj == null);
        Assert.IsTrue(board.GetOccupier(QUEEN_TILE) == null);
        Assert.AreSame(queen, board.GetOccupier(PAWN_TILE));
        Assert.AreSame(queen.GetCurrentTile(), board.GetBoardSquare(PAWN_TILE));
        Assert.IsFalse(game.CurrentPlayer.Units.Contains(pawnObj));
        Assert.IsTrue(game.OtherPlayer.Units.Count == 2);
        Assert.IsTrue(game.CurrentPlayer.Units.Count == 1);
    }

    [UnityTest]
    public IEnumerator DestroyUnit_PawnAttackQueen()
    {
        yield return null;
    }
}