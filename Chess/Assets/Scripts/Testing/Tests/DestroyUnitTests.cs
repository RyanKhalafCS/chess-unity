using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
public class DestroyUnitTests
{

    private TestGameFactory testGameFactory;

    public DestroyUnitTests()
    {
        this.testGameFactory = new TestGameFactory();
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
        //SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        //yield return null;
        /*ARRANGE*/
        yield return SceneManager.LoadSceneAsync(
            "SampleScene",
            LoadSceneMode.Single);

        GameObject queenObj = new("White Queen");
        Queen queen = queenObj.AddComponent<Queen>();
        const int QUEEN_TILE = 5;

        GameObject pawnObj = new("Black Pawn");
        Pawn pawn = pawnObj.AddComponent<Pawn>();
        const int PAWN_TILE = 33;

        GameManager gm = testGameFactory.CreateTwoPieceGame(
            queen, pawn, QUEEN_TILE, PAWN_TILE, 
            TestGameFactory.WHITE_KING_LOC, TestGameFactory.BLACK_KING_LOC);
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

        yield return null;

    }

    [UnityTest]
    public IEnumerator DestroyUnit_PawnAttackQueen()
    {
        yield return null;
    }
}