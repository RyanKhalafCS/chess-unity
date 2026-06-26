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
        if(ActualOccupier != null)
            notMovingPlayer.Units.Remove(ActualOccupier.GetObj());
        //Check check
        badMove = game.Rules.IsInCheck(notMovingPlayer, movingPlayer);

        //Debug.Log("np = " + newPiece + ", " + newPieceActualTile.SquareNumber + "\nActual occ = " + ActualOccupier + ", " + moveSquare);
        //undo move
        game.Board.UnmovePiece(newPiece, newPieceActualTile.SquareNumber, ActualOccupier, moveSquare);

        //Debug.Log("np = " + newPiece + ", " + newPieceActualTile.SquareNumber + "\nActual occ = " + ActualOccupier + ", " + moveSquare);
        //Debug.Log("occ 4 = " + move + ", " + game.Board.GetOccupier(move.SquareNumber));
        if (ActualOccupier != null)
            notMovingPlayer.Units.Add(ActualOccupier.GetObj());
        //reset unit's moves//USE FUNCTION
        game.UpdateMoveLists();
        //for (int i = 0; i < notMovingPlayer.Units.Count; i++)
        //{
        //    IGamePiece enemyPiece = notMovingPlayer.Units[i].GetComponent<IGamePiece>();
        //    enemyPiece.SetPossibleMoves();
        //}
        //for (int i = 0; i < movingPlayer.Units.Count; i++)
        //{
        //    IGamePiece piece = movingPlayer.Units[i].GetComponent<IGamePiece>();
        //    piece.SetPossibleMoves();
        //}

        //Debug.Log("occ 5 = " + move + ", " + game.Board.GetOccupier(move.SquareNumber));

        //Debug.Log("np = " + newPiece + ", " + newPieceActualTile.SquareNumber + "\nActual occ = " + ActualOccupier + ", " + moveSquare);
        return badMove;
    }




    /*
     */

    /*
     
        game.Board.SetOccupier(null, newPieceActualTile.SquareNumber);//newPieceActualTile.Occupier = null;
        game.Board.SetOccupier(newPiece, moveSquare);//move.Occupier = newPiece;
        newPiece.SetCurrentTile(move);
     */
    /*
     //for (int i = 0; i < notMovingPlayer.Units.Count; i++)
        //{
        //    IGamePiece EnemyPiece = notMovingPlayer.Units[i].GetComponent<IGamePiece>();
        //    if (EnemyPiece == ActualOccupier)
        //        continue;
        //    EnemyPiece.SetPossibleMoves();
        //    if (newPiece is King && EnemyPiece.GetPossibleAttacks().Contains(move))
        //    {
        //        badMove = true;
        //    }
        //    else if (EnemyPiece.GetPossibleAttacks().Contains(movingPlayer.PlayersKing.GetCurrentTile()))
        //    {
        //        badMove = true;
        //    }
        //}
        /* END CHECK IF MOVE ENDANGERS KING */
        /* UNDO MOVE SWAP */
        //newPieceActualTile.Occupier = newPiece;


        //Debug.Log("CHECKING>>>");
        //King enemyKing = notMovingPlayer.PlayersKing;
        //enemyKing.SetPossibleMoves();
        //for(int i = 0; i < enemyKing.GetPossibleAttacks().Count; i++)
        //{
        //    Debug.Log("attack tile = " + enemyKing.GetPossibleAttacks()[i]);
        //}
        //if (newPiece is King && enemyKing.GetPossibleAttacks().Contains(movingPlayer.PlayersKing.GetCurrentTile()))//Contains(move))
        //    badMove = true;
        //Debug.Log("DONE CHECKING>>>");

    //game.Board.SetOccupier(newPiece, newPieceActualTile.SquareNumber);
        //newPiece.SetCurrentTile(newPieceActualTile);
        //game.Board.SetOccupier(ActualOccupier, moveSquare);
        //if (hadOccupier)
        //{
        //    ActualOccupier.SetCurrentTile(move);
        //}

        //for (int i = 0; i < notMovingPlayer.Units.Count; i++)
        //{
        //    IGamePiece enemyPiece = notMovingPlayer.Units[i].GetComponent<IGamePiece>();
        //    enemyPiece.SetPossibleMoves();
        //}
        /* END UNDO MOVE SWAP */
        //if(badMove)
        //    Debug.Log("King would be in check. Move must protect king.");
}
