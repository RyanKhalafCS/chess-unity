using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MoveTests
{

    private TestGameFactory _testGameFactory;

    public MoveTests()
    {
        _testGameFactory = new TestGameFactory();
    }



    [UnityTest]
    public IEnumerator MoveTest_SimplePass()
    {
        /*ARRANGE*/
        TestRuntime.IsTesting = true;
        yield return SceneManager.LoadSceneAsync(
            "SampleScene",
            LoadSceneMode.Single);
        yield return null;

        GameObject queenObj = new("White Queen");
        Queen queen = queenObj.AddComponent<Queen>();
        const int QUEEN_TILE = 5;

        GameManager gm = _testGameFactory.CreateSinglePieceGame(
            queen, Color.White, QUEEN_TILE,
            TestGameFactory.WHITE_KING_LOC, TestGameFactory.BLACK_KING_LOC);
        Game game = gm.Game;
        BoardPlane board = game.Board;

        int moveToTile = 45;
        BoardSquare tile = board.GetBoardSquare(moveToTile);

        /*ACT*/
        gm.RequestMove(queenObj, tile);
        yield return null;

        /*ASSERT*/
        Assert.IsTrue(board.GetOccupier(QUEEN_TILE) == null);
        Assert.AreSame(queen.GetCurrentTile(), tile);
        Assert.AreSame(board.GetOccupier(moveToTile), queen);
        yield return null;
    }
}