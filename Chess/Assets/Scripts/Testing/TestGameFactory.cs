using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static log4net.Appender.ColoredConsoleAppender;
using static UnityEditor.FilePathAttribute;

internal class TestGameFactory
{
    public const int WHITE_KING_LOC = 4;
    public const int BLACK_KING_LOC = 60;
    public TestGameFactory() { }
    //TEST GAMES WILL ALWAYS INCLUDE A KING

    
    //maybe improve this at some point
    public void BuildFullLists(
        List<IGamePiece> pieces, 
        List<Color> colors, 
        List<int> locations)
    {
        int tile = 0;
        for (int i = 0; i < 2; i++)
        {
            Color color;
            if (i == 1)
            {
                color = Color.Black;
                tile = 48;
                for (int j = 0; j < 8; j++)
                {
                    GameObject pawnObj = new();
                    Pawn pawn = pawnObj.AddComponent<Pawn>();
                    tile++;
                    pawn.FirstMove = false;
                    pieces.Add(pawn);
                    colors.Add(color);
                    locations.Add(tile);
                }
            }
            else
                color = Color.White;

            GameObject rookObjR = new();
            Rook rookR = rookObjR.AddComponent<Rook>();
            tile++;
            pieces.Add(rookR);
            colors.Add(color);
            locations.Add(tile);

            GameObject knightRObj = new();
            Knight knightR = knightRObj.AddComponent<Knight>();
            tile++;
            pieces.Add(knightR);
            colors.Add(color);
            locations.Add(tile);

            GameObject bishopRObj = new();
            Bishop bishopR = bishopRObj.AddComponent<Bishop>();
            tile++;
            pieces.Add(bishopR);
            colors.Add(color);
            locations.Add(tile);

            GameObject queenObj = new();
            Queen queen = queenObj.AddComponent<Queen>();
            tile++;
            pieces.Add(queen);
            colors.Add(color);
            locations.Add(tile);

            //king happens elsewhere

            GameObject bishopLObj = new();
            Bishop bishopL = bishopLObj.AddComponent<Bishop>();
            tile += 2;
            pieces.Add(bishopL);
            colors.Add(color);
            locations.Add(tile);

            GameObject knightLObj = new();
            Knight knight = knightLObj.AddComponent<Knight>();
            tile++;
            pieces.Add(knight);
            colors.Add(color);
            locations.Add(tile);

            GameObject rookObjL = new();
            Rook rookL = rookObjL.AddComponent<Rook>();
            tile++;
            pieces.Add(rookL);
            colors.Add(color);
            locations.Add(tile);

            if (color != Color.White)
                continue;
            for (int j = 0; j < 8; j++)
            {
                GameObject pawnObj = new();
                Pawn pawn = pawnObj.AddComponent<Pawn>();
                tile++;
                pawn.FirstMove = false;
                pieces.Add(pawn);
                colors.Add(color);
                locations.Add(tile);
            }
        }
    }

    public void SetupPiece(IGamePiece piece, int tileNum, BoardPlane board)
    {
        BoardSquare pieceTile =
            board.GetBoardSquare(tileNum);
        piece.SetCurrentTile(pieceTile);
        board.SetOccupier(piece, tileNum);
    }

    public GameManager CreateEmptyGame(int whiteKingLoc, int blackKingLoc)
    {
        // Create game
        GameObject gmObj = GameObject.FindGameObjectWithTag("Game Manager");
        //Assert.IsNotNull(gmObj, "Couldn't find Game Manager.");
        GameManager gm = gmObj.GetComponent<GameManager>();
        //gm.Initialize(true);
        Game game = gm.Game;
        BoardPlane board = gm.Board;

        King whiteKing = game.CurrentPlayer.PlayersKing;
        whiteKing.Initialize(board);
        whiteKing.TestInit(Color.White);
        King blackKing = game.OtherPlayer.PlayersKing;
        blackKing.Initialize(board);
        blackKing.TestInit(Color.Black);

        SetupPiece(whiteKing, whiteKingLoc, board);
        SetupPiece(blackKing, blackKingLoc, board);

        return gm;
    }


    //remove at some point
    public GameManager CreateSinglePieceGame(IGamePiece piece, Color color, int location, int whiteKingLoc, int blackKingLoc)
    {
        GameManager gm = CreateEmptyGame(whiteKingLoc, blackKingLoc);
        Game game = gm.Game;
        BoardPlane board = game.Board;

        // Create pieces
        piece.Initialize(board);
        piece.TestInit(color);

        //Setup game
        SetupPiece(piece, location, board);
        if(color == Color.White)
            game.CurrentPlayer.Units.Add(piece.GetObj());
        else
            game.OtherPlayer.Units.Add(piece.GetObj());

        return gm;
    }


    //MAKE IT SO IT ALSO TAKES PARAM COLOR, SO THAT CAN TEST CASE SUCH AS MOVE(PIECE, FRIENDLY PIECE) BETTER
    public GameManager CreateNPieceGame(List<IGamePiece> pieces, List<Color> colors, List<int> locations, int whiteKingLoc, int blackKingLoc)
    {
        GameManager gm = CreateEmptyGame(whiteKingLoc, blackKingLoc);
        Game game = gm.Game;
        BoardPlane board = game.Board;

        if (pieces.Count != colors.Count || pieces.Count != locations.Count)
        {
            return null;
        }

        // Create pieces
        for(int i = 0; i < pieces.Count; i++)
        {
            pieces[i].Initialize(board);
            pieces[i].TestInit(colors[i]);

            // Place piece on square
            SetupPiece(pieces[i], locations[i], board);

            // Register pieces with players
            GameObject obj = pieces[i].GetObj();
            if (colors[i] == game.CurrentPlayer.PlayerColor)
                game.CurrentPlayer.Units.Add(obj);
            else
                game.OtherPlayer.Units.Add(obj);
            obj.AddComponent<SelectPiece>();
            if (obj.TryGetComponent<SelectPiece>(out SelectPiece sp))
            {
                if (colors[i] == Color.White)
                    sp.enabled = true;
                else
                    sp.enabled = false;
            }
        }

        King whiteKing = game.CurrentPlayer.PlayersKing;
        whiteKing.Initialize(board);
        whiteKing.TestInit(whiteKing.GetColor());
        whiteKing.GetObj().AddComponent<SelectPiece>().enabled = true;
        King blackKing = game.OtherPlayer.PlayersKing;
        blackKing.Initialize(board);
        blackKing.TestInit(blackKing.GetColor());
        blackKing.GetObj().AddComponent<SelectPiece>().enabled = false;

        return gm;
    }


    public Game CreatePinnedPieceScenario()
    {
        return null;
    }
    //test to ensure PrepareMove(), Move(), and Unmove() all work as expected together when called inside MoveInCheck()
    public Game CreateCaptureScenario(int whiteKingLoc, int blackKingLoc)
    {
        GameManager gm = CreateEmptyGame(whiteKingLoc, blackKingLoc);
        Game game = gm.Game;
        BoardPlane board = game.Board;

        // Create pieces
        GameObject kingObj = new GameObject("White King");
        King king = kingObj.AddComponent<King>();
        king.Initialize(board);
        king.TestInit(Color.White);
        GameObject rookObj = new GameObject("White Rook");
        Rook rook = rookObj.AddComponent<Rook>();
        rook.Initialize(board);
        rook.TestInit(Color.White);
        GameObject bishopObj = new GameObject("Black Bishop");
        Bishop bishop = bishopObj.AddComponent<Bishop>();
        bishop.Initialize(board);
        bishop.TestInit(Color.Black);

        // Place whiteKing on square 60
        const int kingTile = 60;
        SetupPiece(king, kingTile, board);

        // Place rook on square 46
        const int rookTile = 46;
        SetupPiece(rook, rookTile, board);

        // Place bishop on square 19
        const int bishopTile = 19;
        SetupPiece(bishop, bishopTile, board);

        // Register pieces with players
        game.CurrentPlayer.Units.Add(kingObj);
        game.CurrentPlayer.Units.Add(rookObj);
        game.OtherPlayer.Units.Add(bishopObj);

        return game;
    }

}