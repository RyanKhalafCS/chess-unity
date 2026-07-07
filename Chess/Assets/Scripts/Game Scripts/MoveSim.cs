using System.Collections.Generic;
using UnityEngine;

public class MoveSim
{
    /*
     * Ensures the CurrentPlayer cannot make a move that would put them in check.
     * 
     * >record actualOccupier of moveSquare
     * >set occupier of moveSquare to null
     * >Set actualOccupier.CurrentTile() to null
     * 
     * >record newPiece's actual tile
     * >move piece
     */
    public bool MoveInCheck(IGamePiece newPiece, BoardSquare move, Game game)
    {
        //get players
        Player movingPlayer;
        Player notMovingPlayer;
        {
            movingPlayer =       //player being tested for self checking
            game.CurrentPlayer.PlayerColor == newPiece.GetColor()
            ? game.CurrentPlayer
            : game.OtherPlayer;

            notMovingPlayer =    //OtherPlayer
                (movingPlayer == game.CurrentPlayer)
                ? game.OtherPlayer
                : game.CurrentPlayer;
        }
        
        int moveSquare = move.SquareNumber;
        bool badMove = false;
        //Debug.Log("occ 1 = " + move + ", " + game.Board.GetOccupier(move.SquareNumber));
        //freeing up move location
        IGamePiece ActualOccupier = game.Board.PrepareMove(moveSquare);
        //Debug.Log("occ 2 = " + move + ", " + game.Board.GetOccupier(move.SquareNumber));

        BoardSquare newPieceActualTile = newPiece.GetCurrentTile();

        //do move
        game.Board.MovePiece(newPiece, move.SquareNumber);
        //Debug.Log("occ 3 = " + move + ", " + game.Board.GetOccupier(move.SquareNumber));

        //reset unit's moves
        for (int i = 0; i < notMovingPlayer.Units.Count; i++)
        {
            IGamePiece enemyPiece = notMovingPlayer.Units[i].GetComponent<IGamePiece>();
            if(enemyPiece.GetCurrentTile() == null)
            {
                continue;
            }
            if (game.Board.GetOccupier(enemyPiece.GetCurrentTile().SquareNumber) != enemyPiece)//exclude from loop the piece who was captured in the simulation
                continue;
            enemyPiece.SetPossibleMoves();
        }

        bool removedEnemyUnit = ActualOccupier != null
            && notMovingPlayer.Units.Remove(ActualOccupier.GetObj());
        bool movingCheckBefore = movingPlayer.Check;
        List<IGamePiece> checkersBefore = new(notMovingPlayer.UnitsCheckingEnemyKing);

        //Check check
        //if (game.TestGame)
        //    badMove = game.Rules.IsInCheck(movingPlayer, notMovingPlayer);
        //else
            badMove = game.Rules.IsInCheck(notMovingPlayer, movingPlayer);//maybe write it so that both players are checked for check

        //Debug.Log("np = " + newPiece + ", " + newPieceActualTile.SquareNumber + "\nActual occ = " + ActualOccupier + ", " + moveSquare);
        //undo move
        game.Board.UnmovePiece(newPiece, newPieceActualTile.SquareNumber, ActualOccupier, moveSquare);

        //Debug.Log("np = " + newPiece + ", " + newPieceActualTile.SquareNumber + "\nActual occ = " + ActualOccupier + ", " + moveSquare);
        //Debug.Log("occ 4 = " + move + ", " + game.Board.GetOccupier(move.SquareNumber));
        if (removedEnemyUnit)//only re-add if it was genuinely removed from notMovingPlayer above
            notMovingPlayer.Units.Add(ActualOccupier.GetObj());
        //reset unit's moves//USE FUNCTION
        game.UpdateMoveLists();

        // Restore the real check-state the simulation clobbered above.
        movingPlayer.SetInCheck(movingCheckBefore);
        notMovingPlayer.UnitsCheckingEnemyKing = checkersBefore;

        //Debug.Log("occ 5 = " + move + ", " + game.Board.GetOccupier(move.SquareNumber));

        //Debug.Log("np = " + newPiece + ", " + newPieceActualTile.SquareNumber + "\nActual occ = " + ActualOccupier + ", " + moveSquare);
        return badMove;
    }
}
