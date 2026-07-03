using UnityEngine;

internal class TestGameFactory
{
    public const int WHITE_KING_LOC = 4;
    public const int BLACK_KING_LOC = 60;
    public TestGameFactory() { }
    //TEST GAMES WILL ALWAYS INCLUDE A KING

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
        gm.Initialize(true);
        Game game = gm.Game;
        BoardPlane board = gm.Board;

        King whiteKing = game.CurrentPlayer.PlayersKing;
        whiteKing.Initialize(board);
        whiteKing.TestInit(whiteKing.GetColor());
        King blackKing = game.OtherPlayer.PlayersKing;
        blackKing.Initialize(board);
        blackKing.TestInit(blackKing.GetColor());

        SetupPiece(whiteKing, whiteKingLoc, board);
        SetupPiece(blackKing, blackKingLoc, board);

        return gm;
    }



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
    public GameManager CreateTwoPieceGame(IGamePiece pieceA, IGamePiece pieceB, 
        int locationA, int locationB, int whiteKingLoc, int blackKingLoc)
    {
        GameManager gm = CreateEmptyGame(whiteKingLoc, blackKingLoc);
        Game game = gm.Game;
        BoardPlane board = game.Board;

        // Create pieces
        pieceA.Initialize(board);
        pieceA.TestInit(Color.White);
        pieceB.Initialize(board);
        pieceB.TestInit(Color.Black);
        King whiteKing = game.CurrentPlayer.PlayersKing;
        whiteKing.Initialize(board);
        whiteKing.TestInit(whiteKing.GetColor());
        King blackKing = game.OtherPlayer.PlayersKing;
        blackKing.Initialize(board);
        blackKing.TestInit(blackKing.GetColor());

        // Place pieces on square
        SetupPiece(pieceA, locationA, board);
        SetupPiece(pieceB, locationB, board);

        // Register pieces with players
        game.CurrentPlayer.Units.Add(pieceA.GetObj());
        game.OtherPlayer.Units.Add(pieceB.GetObj());

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