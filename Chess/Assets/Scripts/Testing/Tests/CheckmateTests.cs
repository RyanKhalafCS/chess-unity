using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class CheckmateTests
{

    private TestGameFactory _testGameFactory;

    public CheckmateTests()
    {
        _testGameFactory = new TestGameFactory();
    }

    [UnityTest]
    public IEnumerator CheckmateTest_NoCheckmateKingCanEscape()
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
        const int QUEEN_TILE = 52;

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
        bool checkmate = game.Rules.IsCheckmate();

        /*ASSERT*/
        Assert.IsTrue(check);
        Assert.IsFalse(checkmate);
    }


    [UnityTest]
    public IEnumerator CheckmateTest_NoCheckmateCanPathBlock()
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

        GameObject rookObjA = new("White Rook");
        Rook rookA = rookObjA.AddComponent<Rook>();
        Color rColorA = Color.White;
        const int ROOK_TILE_A = 1;
        pieces.Add(rookA);
        colors.Add(rColorA);
        locations.Add(ROOK_TILE_A);

        GameObject rookObjB = new("White Rook");
        Rook rookB = rookObjB.AddComponent<Rook>();
        Color rColorB = Color.White;
        const int ROOK_TILE_B = 2;
        pieces.Add(rookB);
        colors.Add(rColorB);
        locations.Add(ROOK_TILE_B);

        GameObject knightObj = new("Black Knight");
        Knight knight = knightObj.AddComponent<Knight>();
        Color kColor = Color.Black;
        const int KNIGHT_TILE = 43;
        pieces.Add(knight);
        colors.Add(kColor);
        locations.Add(KNIGHT_TILE);

        GameManager gm = _testGameFactory.CreateNPieceGame(
            pieces, colors, locations, 
            TestGameFactory.WHITE_KING_LOC, 57);

        Game game = gm.Game;
        BoardPlane board = game.Board;

        rookA.SetPossibleMoves();
        rookB.SetPossibleMoves();
        knight.SetPossibleMoves();
        game.OtherPlayer.PlayersKing.SetPossibleMoves();

        /*ACT*/
        bool check = game.Rules.IsInCheck(game.CurrentPlayer, game.OtherPlayer);
        bool checkmate = game.Rules.IsCheckmate();

        /*ASSERT*/
        Assert.IsTrue(check);
        Assert.IsFalse(checkmate);
    }

    
    [UnityTest]
    public IEnumerator CheckmateTest_NoCheckmateCanDestroyAttacker()
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
        const int QUEEN_TILE = 52;

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
        bool checkmate = game.Rules.IsCheckmate();

        /*ASSERT*/
        Assert.IsTrue(check);
        Assert.IsFalse(checkmate);
    }


    [UnityTest]
    public IEnumerator CheckmateTest_IsCheckmateSingleAttacker()
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

    [UnityTest]
    public IEnumerator CheckmateTest_IsCheckmateTwoAttackers()
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
