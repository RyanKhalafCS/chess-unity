using UnityEngine;

public class Movement
{
    public enum MoveDir //maybe remove
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }
    public const int NEXT_N = 8;
    public const int NEXT_NW = 9;
    public const int NEXT_NE = 7;
    public const int NEXT_W = 1;
    public const int NEXT_E = -1;
    public const int NEXT_S = -8;
    public const int NEXT_SW = -7;
    public const int NEXT_SE = -9;
    public const int NUM_OF_DIRS = 8;

    


    public static bool IsOverEdge(int squareNum, int dir)
    {
        int index = squareNum - 1;
        int row = index / 8;
        int col = index % 8;

        if(dir == NEXT_N && (row == 7 ))
            return true;
        if(dir == NEXT_S && (row == 0))
            return true;
        if(dir == NEXT_E && (col == 0))
            return true;
        if(dir == NEXT_W && (col == 7 ))
            return true;
        if(dir == NEXT_NE && (col == 0 || row == 7))
            return true;
        if(dir == NEXT_NW && (col == 7 || row == 7))
            return true;
        if(dir == NEXT_SW && (col == 7 || row == 0))
            return true;
        if(dir == NEXT_SE && (col == 0 || row == 0))
            return true;

        return false;
    }

    public static int GetColorMultiplier(Player.Color Color)
    {
        if(Color == Player.Color.White)
            return 1;
        else
            return -1;
    }

    /* public static BoardSquare CanKingRookSwap(IGamePiece obj)
    {
        King kingPiece = null;
        Rook rookPiece = null;
        if(obj is King king)
        {
            kingPiece = king;
        }
        else
        {
            rookPiece = (Rook) obj;
        }
        if(kingPiece != null && kingPiece.firstMove)//MOVE CONTENTS INTO HELPER FUNCTION PROBLY.
        {
            int currTile = kingPiece.GetCurrentTile().squareNumber;
            int squareNum1 = currTile - 1 - 1;
            int squareNum2 = currTile - 2 - 1;
            int squareNum3 = currTile - 3 - 1;
            BoardSquare tile1 = BoardPlane.board[squareNum1/8, squareNum1%8];
            BoardSquare tile2 = BoardPlane.board[squareNum2/8, squareNum2%8];
            BoardSquare tile3 = BoardPlane.board[squareNum3/8, squareNum3%8];
            Debug.Log("squareNumber king swap = " + kingPiece.GetCurrentTile().squareNumber + ", " + kingPiece.GetCurrentTile().occupier + ", " + tile3.occupier);
            Rook rooks = (Rook)tile3.occupier;
            if(rooks.firstMove)
            {
                if(tile1.occupier == null && tile2.occupier == null)
                { 
                    return tile2;
                }
            }
        }
        else if(rookPiece != null && rookPiece.firstMove)
        {
            int currTile = rookPiece.GetCurrentTile().squareNumber;
            int squareNum1 = currTile + 1 - 1;
            int squareNum2 = currTile + 2 - 1;
            int squareNum3 = currTile + 3 - 1;
            BoardSquare tile1 = BoardPlane.board[squareNum1/8, squareNum1%8];
            BoardSquare tile2 = BoardPlane.board[squareNum2/8, squareNum2%8];
            BoardSquare tile3 = BoardPlane.board[squareNum3/8, squareNum3%8];
            Debug.Log("squareNumber rook swap = " + rookPiece.GetCurrentTile().squareNumber + ", " + rookPiece.GetCurrentTile().occupier + ", " + tile3.occupier);
            King kings = (King)tile3.occupier;
            if(kings.firstMove)
            {
                if(tile1.occupier == null && tile2.occupier == null)
                { 
                    return tile1;
                }
            }
        }
        return null;
    } */

/* 
    
    public static void KingRookSwap(IGamePiece gPiece)//readonly/const/?
    {
        King kingPiece = null;
        Rook rookPiece = null;
        if(gPiece is King king)
        {
            kingPiece = king;
        }
        else
        {
            rookPiece = (Rook)gPiece;
        }
        if(kingPiece != null && kingPiece.firstMove)//move to helper function
        {
            int currTile = rookPiece.GetCurrentTile().squareNumber;
            int squareNum1 = currTile + 1 - 1;
            int squareNum2 = currTile + 2 - 1;
            int squareNum3 = currTile + 3 - 1;
            BoardSquare tile1 = BoardPlane.board[squareNum1/8, squareNum1%8];
            BoardSquare tile2 = BoardPlane.board[squareNum2/8, squareNum2%8];
            BoardSquare tile3 = BoardPlane.board[squareNum3/8, squareNum3%8];
            King kings = (King)tile3.occupier;

            kingPiece.GetCurrentTile().occupier = null;
            rookPiece.GetCurrentTile().occupier = null;
            kingPiece.SetCurrentTile(tile2);
            rookPiece.SetCurrentTile(tile1);
            kingPiece.GetCurrentTile().occupier = this;
            rookPiece.GetCurrentTile().occupier = rookPiece;
        }
        else
        {
            int currTile = rookPiece.GetCurrentTile().squareNumber;
            int squareNum1 = currTile + 1 - 1;
            int squareNum2 = currTile + 2 - 1;
            int squareNum3 = currTile + 3 - 1;
            BoardSquare tile1 = BoardPlane.board[squareNum1/8, squareNum1%8];
            BoardSquare tile2 = BoardPlane.board[squareNum2/8, squareNum2%8];
            BoardSquare tile3 = BoardPlane.board[squareNum3/8, squareNum3%8];
            King kings = (King)tile3.occupier;

            kingPiece.GetCurrentTile().occupier = null;
            rookPiece.GetCurrentTile().occupier = null;
            kingPiece.SetCurrentTile(tile2);
            rookPiece.SetCurrentTile(tile1);
            kingPiece.GetCurrentTile().occupier = this;
            rookPiece.GetCurrentTile().occupier = rookPiece;
        }
    }
    */ 
}

/*IGamePiece ActualOccupier = BoardPlane.board[squareNum/8, squareNum%8].occupier;
        BoardPlane.board[squareNum/8, squareNum%8].occupier = testPiece;
        testPiece.GetCurrentTile().occupier = null;
        bool badMove = false;
        for(int i = 0; i < Game.OtherPlayer.Units.Count; i++)
        {
            IGamePiece EnemyPiece = Game.OtherPlayer.Units[i].GetComponent<IGamePiece>();
            EnemyPiece.SetPossibleMoves();
            if(EnemyPiece.GetPossibleAttacks().Contains(Game.OtherPlayer.PlayersKing.GetCurrentTile()))//BoardPlane.board[squareNum/8, squareNum%8]))
            {
                badMove = true;
            }
        }
        BoardPlane.board[squareNum/8, squareNum%8].occupier = ActualOccupier;
        testPiece.GetCurrentTile().occupier = testPiece;
        //set each pieces possible moves back now that the actual occupier is back*/