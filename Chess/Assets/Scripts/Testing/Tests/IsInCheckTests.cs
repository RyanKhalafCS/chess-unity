using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class IsInCheckTests
{

    private TestGameFactory _testGameFactory;

    public IsInCheckTests()
    {
        _testGameFactory = new TestGameFactory();
    }


    [UnityTest]
    public IEnumerator IsInCheckTest_NoCheck()
    {
        /*ARRANGE*/
        TestRuntime.IsTesting = true;
        yield return SceneManager.LoadSceneAsync(
            "SampleScene",
            LoadSceneMode.Single);
        yield return null;

        GameObject queenObj = new("White Queen");
        Queen queen = queenObj.AddComponent<Queen>();
        Color qColor = Color.White;
        const int QUEEN_TILE = 3;

        GameManager gm = _testGameFactory.CreateSinglePieceGame(
            queen, qColor,
            QUEEN_TILE,
            TestGameFactory.WHITE_KING_LOC,
            TestGameFactory.BLACK_KING_LOC);

        Game game = gm.Game;
        BoardPlane board = game.Board;

        queen.SetPossibleMoves();
        game.OtherPlayer.PlayersKing.SetPossibleMoves();

        /*ACT*/
        bool check = game.Rules.IsInCheck(game.CurrentPlayer, game.OtherPlayer);

        /*ASSERT*/
        Assert.IsFalse(check);
    }


    [UnityTest]
    public IEnumerator IsInCheckTest_InCheckNoCheckmate()
    {
        /*ARRANGE*/
        TestRuntime.IsTesting = true;
        yield return SceneManager.LoadSceneAsync(
            "SampleScene",
            LoadSceneMode.Single);
        yield return null;

        GameObject queenObj = new("White Queen");
        Queen queen = queenObj.AddComponent<Queen>();
        Color qColor = Color.White;
        const int QUEEN_TILE = 44;

        GameManager gm = _testGameFactory.CreateSinglePieceGame(
            queen, qColor,
            QUEEN_TILE,
            TestGameFactory.WHITE_KING_LOC,
            TestGameFactory.BLACK_KING_LOC);

        Game game = gm.Game;
        BoardPlane board = game.Board;

        queen.SetPossibleMoves();
        game.OtherPlayer.PlayersKing.SetPossibleMoves();

        /*ACT*/
        bool check = game.Rules.IsInCheck(game.CurrentPlayer, game.OtherPlayer);

        /*ASSERT*/
        Assert.IsTrue(check);
    }


    [UnityTest]
    public IEnumerator IsInCheckTest_CheckAndCheckmate()
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
        const int QUEEN_TILE = 52;
        pieces.Add(queen);
        colors.Add(qColor);
        locations.Add(QUEEN_TILE);

        GameObject pawnObj = new("White Pawn");
        Pawn pawn = pawnObj.AddComponent<Pawn>();
        Color pColor = Color.White;
        const int PAWN_TILE = 45;
        pawn.FirstMove = false;
        pieces.Add(pawn);
        colors.Add(pColor);
        locations.Add(PAWN_TILE);

        GameManager gm = _testGameFactory.CreateNPieceGame(
            pieces, colors, locations,
            TestGameFactory.WHITE_KING_LOC,
            TestGameFactory.BLACK_KING_LOC);

        Game game = gm.Game;
        BoardPlane board = game.Board;

        queen.SetPossibleMoves();
        pawn.SetPossibleMoves();
        game.OtherPlayer.PlayersKing.SetPossibleMoves();

        /*ACT*/
        game.Rules.IsInCheck(game.CurrentPlayer, game.OtherPlayer);
        bool checkmate = game.Rules.IsCheckmate();

        /*ASSERT*/
        Assert.IsTrue(checkmate);
    }
}
