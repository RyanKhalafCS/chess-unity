using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Testing
{
    internal class TestGameFactory
    {
        public TestGameFactory() { }





        public Game CreateCaptureScenario()
        {
            // Create board
            //BoardPlane board = new BoardPlane();//invalid. cant call constructor for monobehaviour
                                                //For a Unity xUnit/ NUnit test you'd usually do:
            GameObject boardObj = new GameObject();
            BoardPlane board =
                boardObj.AddComponent<BoardPlane>();
            // Create game
            //Game game = new Game(board);
            //or more likely
            Game game = CreateEmptyGame();

            // Remove existing pieces if necessary
            game.WhitePlayer.Units.Clear();
            game.BlackPlayer.Units.Clear();

            // Create pieces
            GameObject bishopObj = new GameObject("White Bishop");
            Bishop bishop = bishopObj.AddComponent<Bishop>();

            GameObject rookObj = new GameObject("Black Rook");
            Rook rook = rookObj.AddComponent<Rook>();

            // Place bishop on square 19
            BoardSquare bishopSquare =
                board.GetBoardSquare(19);

            bishop.SetCurrentTile(bishopSquare);
            board.SetOccupier(bishop, 19);

            // Place rook on square 46
            BoardSquare rookSquare =
                board.GetBoardSquare(46);

            rook.SetCurrentTile(rookSquare);
            board.SetOccupier(rook, 46);

            // Register pieces with players
            game.CurrentPlayer.Units.Add(bishopObj);
            game.OtherPlayer.Units.Add(rookObj);

            return game;
        }

        public Game CreateEmptyGame()
        {
            return null;
        }

    }
}
