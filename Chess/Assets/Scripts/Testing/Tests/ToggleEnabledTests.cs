using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ToggleEnabledTests
{

    private TestGameFactory testGameFactory;

    public ToggleEnabledTests()
    {
        this.testGameFactory = new TestGameFactory();
    }

    [UnityTest]
    public IEnumerator ToggleEnabledTest_SixtyTurnSwapsWithPieceRemoval()
    {
        //ARRANGE
        // ensure GameManager starts in test-mode during scene load
        TestRuntime.IsTesting = true;
        yield return SceneManager.LoadSceneAsync(
            "SampleScene",
            LoadSceneMode.Single);
        yield return null;
    }

        [UnityTest]
    public IEnumerator ToggleEnabledTest_FiftyTurnSwaps()
    {//NEED MORE UNITS IN PLAYER UNIT LISTS.
        //ARRANGE
        // ensure GameManager starts in test-mode during scene load
        TestRuntime.IsTesting = true;
        yield return SceneManager.LoadSceneAsync(
            "SampleScene",
            LoadSceneMode.Single);
        yield return null;

        List<IGamePiece> pieces = new();
        List<Color> colors = new();
        List<int> locations = new();
        List<int> moves = new();
        const int TURNS = 50;

        testGameFactory.BuildFullLists(pieces, colors, locations);

        GameManager gm = testGameFactory.CreateNPieceGame(
            pieces, colors, locations,
            17,
            48);

        Game game = gm.Game;
        BoardPlane board = game.Board;

        King wKing = game.CurrentPlayer.PlayersKing;
        King bKing = game.OtherPlayer.PlayersKing;
        int wMove = wKing.GetCurrentTile().SquareNumber;
        int bMove = bKing.GetCurrentTile().SquareNumber;
        for(int i = 0; i < TURNS; i++)
        {
            if(game.CurrentState == Game.GameState.WhiteTurn)
            {
                if (wMove == 32)
                    wMove += Movement.NEXT_N;
                else
                    wMove++;
                wKing.SetPossibleMoves();
                BoardSquare tile = board.GetBoardSquare(wMove);
                gm.RequestMove(wKing.GetObj(), tile);
            }
            else if (game.CurrentState == Game.GameState.BlackTurn)
            {
                if (bMove == 33)
                    bMove += Movement.NEXT_S;
                else
                    bMove--;
                bKing.SetPossibleMoves();
                BoardSquare tile = board.GetBoardSquare(bMove);
                gm.RequestMove(bKing.GetObj(), tile);
            }
        }

        //ACT
        bool correctWhite = CheckToggle(game, game.CurrentPlayer, true);
        bool correctBlack = CheckToggle(game, game.OtherPlayer, false);

        //ASSERT
        Assert.IsTrue(correctWhite);
        Assert.IsTrue(correctBlack);
    }


    private bool CheckToggle(Game game, Player player, bool currentPlayer)
    {
        bool correct = true;

        for (int i = 0; i < player.Units.Count; i++)
        {
            GameObject unit = player.Units[i];
            if (unit == null) return false;
            if (unit.TryGetComponent<SelectPiece>(out SelectPiece sp))
            {
                if (currentPlayer)
                {
                    if (sp.enabled == false)
                        return false;
                }
                else
                {
                    if (sp.enabled == true)
                        return false;
                }
            }
            else
                return false;
        }

        return correct;
    }

}