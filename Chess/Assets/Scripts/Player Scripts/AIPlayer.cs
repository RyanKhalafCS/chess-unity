using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class AIPlayer : Player
{
    //Player.Color Color;
    Dictionary<AIMove, int> Moves;//Contains the best Use the highest MR move of them all. MAKE A TYPE CALLED Move WITH <myGampiece, int location, int MoveRank>
    //int MoveRank;
    const int pawnKill = 15;
    const int backLineKill = 50;
    const int queenKill = 80;
    const int kingCheck = 70;
    const int checkmateThreshold = 200;

    public AIPlayer(Color color) : base(color)
    {
        //MoveRank = 0;
        Moves = new();
    }


    public void MakeMove()
    {
        /*Rules- The goal is to get the king in check/checkmate. Taking high value pieces (backline units) lowers the difficulty threshold. When lowered sufficiently, go for their king.
        *   >If a back piece would move into a spot thats attackable, -50 MoveRank. If a piece would kill an enemy back piece, +50 MR.
            >Queen is worth 75 MR
            >If a pawn would move into a spot thats attackable, -15 MR. If a pawn can be taken +15 MR.
            >Putting King in Check gives +50 MR.
            >Threshhold = 200.
            >Check is good for setting up to take a piece. Threatening any back piece is good for setting up. The higher the MR 
            >To make "risky" moves a piece needs "insurance" unless a condition is met, for example, the move would set up a high value target.
            >If enemy uses more pawns on one flank, maybe attack that flank.
            >Clear the space between king side rook and build up defense on that side. Only move a pawn on that side to allow a bishop through, pref the one in front of king.
            >If you move something that makes you open, prevent enemy from moving viable unit to exploit vulnerability.
                (if move pawn 4, then space in front of king is open, no real exploit except maybe knight. If move pawn 5, then king is open for bishops/Queen)

            >forethought will take too much time/effort. Or maybe not, is basic version possible? Plan out a set of viable moves until checkmate, but recalc every turn?
            >Maybe program a set of starting combos, then let the algorithm take over. (Knight > squareNum(12), Pawn 4 > )
            >
        */

        //whats a good way? go through each of my units and 
        //for(int i = 0; i < PlayerManager.Players[1].Units.Count; i++)
        //{
        //    IGamePiece currentPiece = PlayerManager.Players[1].Units[i].GetComponent<IGamePiece>();
        //    //Moves.Add(RecursiveFind(currentPiece), MoveRank);
        //    MoveRank = 0;
        //}
    }

    //GetBestMove 
    BoardSquare GetBestMove()
    {
        

        
        return null;
    }

    //
    AIMove RecursiveFind(IGamePiece obj, List<AIMove> list)
    {

        return null;
    }
    
}
public class AIMove
{
    IGamePiece Piece;
    int Loc;//squareNumber
    public AIMove(IGamePiece _piece, int _loc)
    {
        this.Piece = _piece;
        this.Loc = _loc;
    }
}