using System.Collections.Generic;
using UnityEngine;

public class GameRules
{
    private readonly Game game;

    public GameRules(Game game)
    {
        this.game = game;
    }


    public bool CanMove(int squareNum)
    {
        //if (GameManager.Instance.BoardPlane.GetOccupier(squareNum) == null)//PASS BOARD IN THRU CALL
        //    return true;
        //return false;
        return game.Board.GetOccupier(squareNum) == null;
    }


    public bool CanAttack(Player.Color currentPiece, int squareNum)
    {
        IGamePiece atkedPiece = game.Board.GetOccupier(squareNum);//PASS BOARD IN THRU CALL
        //if(atkedPiece != null && currentPiece != atkedPiece.GetColor())
        //    return true;
        //return false;
        return atkedPiece != null && currentPiece != atkedPiece.GetColor();
    }


    //bounds checking needed? Dont think so, either SelectPiece checks or gamePiece.move
    public bool ValidMove(IGamePiece gPiece, BoardSquare move)
    {
        if (move == gPiece.GetCurrentTile())
            return false;
        if (MoveInCheck(gPiece, move))
        {
            Debug.Log("King would be in check. Move must protect king.");
            return false;
        }
        if (gPiece.GetPossibleMoves().Contains(move))
        {
            return true;
        }
        IGamePiece occupier = game.Board.GetOccupier(move.SquareNumber);
        if (gPiece.GetPossibleAttacks().Contains(move) && occupier is not King)
        {
            return true;
        }
        if (gPiece.GetPossibleAttacks().Contains(move) && occupier is King)
        {
            Debug.Log("Can't directly attack a king.");
        }
        return false;
    }



    /*
     * //after each move is made, setPossibleMoves for every game piece. If List.contains(king's boardsquare) then Check = true and maybe add to White/BlackCheck List.

        //check if king is in range of any attack. If so, then next move must remove check from king.
        //So before a piece moves, check for check at the newPosition before move completes. Cannot move in a way to allow check on self.
        //If check is true, then also check for checkmate.
     */
    public void IsInCheck()
    {
        game.CurrentPlayer.UnitsCheckingEnemyKing.Clear();
        for (int i = 0; i < game.CurrentPlayer.Units.Count; i++)
        {
            if (game.CurrentPlayer.Units[i].GetComponent<IGamePiece>().GetPossibleAttacks().Contains(game.OtherPlayer.PlayersKing.GetCurrentTile()))
            {
                game.CurrentPlayer.UnitsCheckingEnemyKing.Add(game.CurrentPlayer.Units[i].GetComponent<IGamePiece>());
            }
        }
        if (game.CurrentPlayer.UnitsCheckingEnemyKing.Count > 0)
        {
            game.OtherPlayer.SetInCheck(true);
        }
        else
            game.OtherPlayer.SetInCheck(false);
    }

    /*
     * Part 1:
     * Check if there is a tile the enemyKing can move to that would make him not in check.
     * For each of enemyKing's possibleMoves, if(!Movement.MoveInCheck()){ checkmate = false }.
     * 
     * RECORD ALL TILES TARGETED WITHIN 1 tile of the king, including the kings tile, as well as the number of times each tile is targeted.
     *  If one of currentPlayer's units getting killed would uncheck king or create an opening, then not checkmate.
     *  
     *  PART 2:
     *  
        for each currentPlayer.unitCheckingEnemyKing, if that unit can be destroyed,
          allowing an opening for king to not be in check, then no checkmate.

        If player has a unit that can kill an otherPlayer's unit that is checking enemy king,
          which would then causes king to uncheck, then checkmate = false.

        if enemyUnit can attack or block enemy piece checking king, !Movement.moveInCheck(move), then not checkmate
        if(enemy king can now move somewhere that isn't in check or enemy king becomes unchecked: then !isCheckmate()
                //(below) && if(OtherPlayer.Units cant move to body block for the king){so get a list of squares between unitcheckingking and king? Better way? move.isvalid() succeeds then king will no longer be in check}
                //simulate move, so OtherPlayer.Units[i] destroys CurrentPlayer.UnitsCheckingEnemyKing[j] and moves to that tile.
     */
    public bool IsCheckmate()
    {
        var current = game.CurrentPlayer;
        Debug.Log($"Check status: {game.OtherPlayer.PlayerColor} in check? {game.OtherPlayer.Check}");
        if (!game.OtherPlayer.Check)
            return false;

        //check if otherPlayer king can escape
        if (CanKingEscape())
        {
            Debug.Log("King can escape!");
            return false;
        }

        //If there are multiple UnitsCheckingEnemyKing, then only a move from the king can escape the checkmate.
        if (game.CurrentPlayer.UnitsCheckingEnemyKing.Count > 1)
        {
            Debug.Log("Several units checking enemy king!");
            return true;
        }

        //check if otherPlayer can destroy unitCheckingEnemyKing
        if (CanDestroyCheckingUnit())
        {
            Debug.Log("Can escape check by attacking!");
            return false;
        }

        //calculate path blocking of unitsCheckingEnemyKing
        if (CanPathBlock())
        {
            Debug.Log("Can escape check by blocking!");
            return false;
        }

        return true;
    }


    //for each of enemyKing's moves, if theres one that can take them out of check then return true.
    private bool CanKingEscape()
    {
        King enemyKing = game.OtherPlayer.PlayersKing;
        List<BoardSquare> toMove = enemyKing.GetPossibleMoves();
        for (int i = 0; i < toMove.Count; i++)
        {
            BoardSquare tile = toMove[i];
            if (ValidMove(enemyKing, tile))
                return true;
        }
        toMove = enemyKing.GetPossibleAttacks();
        for (int i = 0; i < toMove.Count; i++)
        {
            BoardSquare tile = toMove[i];
            if (ValidMove(enemyKing, tile))
                return true;
        }
        return false;
    }


    //
    private bool CanDestroyCheckingUnit()
    {
        for (int i = 0; i < game.OtherPlayer.Units.Count; i++)
        {
            for (int j = 0; j < game.CurrentPlayer.UnitsCheckingEnemyKing.Count; j++)
            {
                if (!game.OtherPlayer.Units[i].TryGetComponent<IGamePiece>(out IGamePiece enemyGamePiece))
                {
                    continue;
                }
                BoardSquare tileToAttack = game.CurrentPlayer.UnitsCheckingEnemyKing[j].GetCurrentTile();
                if (!enemyGamePiece.GetPossibleAttacks().Contains(tileToAttack))
                {
                    continue;
                }
                if (ValidMove(enemyGamePiece, tileToAttack))
                {
                    return true;
                }
            }
        }
        return false;
    }


    //calculate the diagonal, vertical, and horizontal paths. If a piece can move onto that path, then not checkmate.
    private bool CanPathBlock()
    {
        List<int> paths = new();
        //Queen moves are added to possibleMoves in order N, S, E, W, NW, NE, SW, SE
        //Bishop moves are added in order NW, NE, SW, SE

        for (int i = 0; i < game.CurrentPlayer.UnitsCheckingEnemyKing.Count; i++)
        {
            IGamePiece unitCheckingEnemy = game.CurrentPlayer.UnitsCheckingEnemyKing[i];
            GetPath(paths, unitCheckingEnemy);
        }
        //if OtherPlayer can move a unit into the path of CurrentPlayer.UnitsCheckingEnemyKing, then not checkmate.
        for (int i = 0; i < game.OtherPlayer.Units.Count; i++)
        {
            IGamePiece unitDefending = game.OtherPlayer.Units[i].GetComponent<IGamePiece>();
            for (int j = 0; j < paths.Count; j++)
            {
                BoardSquare tile = game.Board.GetBoardSquare(paths[j]);
                if (ValidMove(unitDefending, tile))//unitDefending.GetPossibleMoves().Contains(tile) && 
                {
                    return true;
                }
            }
        }
        return false;
    }


    private void GetPath(List<int> paths, IGamePiece unitAttacking)
    {
        int dir = GetDirection(unitAttacking);
        if (dir == ExceptionHandler.SEVERE_EXCEPTION)
            return;

        int tile = unitAttacking.GetCurrentTile().SquareNumber + dir;
        int enemyKingTile = game.OtherPlayer.PlayersKing.GetComponent<IGamePiece>().GetCurrentTile().SquareNumber;
        while (tile != enemyKingTile)
        {
            paths.Add(tile);
            tile += dir;
        }
    }

    private int GetDirection(IGamePiece piece)
    {
        int tileAttacker = piece.GetCurrentTile().SquareNumber;
        int enemyKingTile =
            game.OtherPlayer.PlayersKing.GetComponent<IGamePiece>()
            .GetCurrentTile().SquareNumber;

        int[] directions =
        {
            Movement.NEXT_N,
            Movement.NEXT_NW,
            Movement.NEXT_NE,
            Movement.NEXT_W,
            Movement.NEXT_E,
            Movement.NEXT_S,
            Movement.NEXT_SW,
            Movement.NEXT_SE
        };

        foreach (int dir in directions)
        {
            int squareNum = tileAttacker + dir;
            while (BoardPlane.InBounds(squareNum) && !Movement.IsOverEdge(squareNum - dir, dir))
            {
                if (squareNum == enemyKingTile)
                    return dir;
                else if (game.Board.GetOccupier(squareNum) != null)     //(BoardPlane.GetBoardSquare(squareNum).Occupier != null)
                    break;
                squareNum += dir;
            }
        }
        return ExceptionHandler.SEVERE_EXCEPTION;
    }



    /*
     * Ensures the CurrentPlayer cannot make a move that would put them in check.
     */
    public bool MoveInCheck(IGamePiece newPiece, BoardSquare move)
    {
        Player movingPlayer;
        Player notMovingPlayer;
        //Game game = GameManager.Instance.Game;
        //BoardPlane board = GameManager.Instance.BoardPlane;
        if (newPiece.GetColor() == Player.Color.White)
        {
            if (game.CurrentPlayer.PlayerColor == Player.Color.White)
            {
                movingPlayer = game.CurrentPlayer; //Game.CurrentPlayer;
                notMovingPlayer = game.OtherPlayer;
            }
            else
            {
                movingPlayer = game.OtherPlayer;
                notMovingPlayer = game.CurrentPlayer;
            }
        }
        else
        {
            if (game.CurrentPlayer.PlayerColor == Player.Color.Black)
            {
                movingPlayer = game.CurrentPlayer;
                notMovingPlayer = game.OtherPlayer;
            }
            else
            {
                movingPlayer = game.OtherPlayer;
                notMovingPlayer = game.CurrentPlayer;
            }
        }
        /*~~MOVE/SWAP BEGINS~~*/
        IGamePiece ActualOccupier = null;
        int moveSquare = move.SquareNumber;
        bool hadOccupier =
            (game.Board.GetOccupier(moveSquare) is null) ? false : true;
        bool badMove = false;
        if (hadOccupier)
        {
            ActualOccupier = game.Board.GetOccupier(moveSquare);//move.Occupier;
            game.Board.SetOccupier(null, moveSquare);
            ActualOccupier.SetCurrentTile(null);
        }
        BoardSquare newPieceActualTile = newPiece.GetCurrentTile();
        //BoardPlane.Instance.MovePiece(newPiece, move.SquareNumber);
        game.Board.SetOccupier(null, newPieceActualTile.SquareNumber);//newPieceActualTile.Occupier = null;
        game.Board.SetOccupier(newPiece, moveSquare);//move.Occupier = newPiece;
        newPiece.SetCurrentTile(move);
        /*~~MOVE/SWAP ENDS~~*/
        /* CHECK IF MOVE ENDANGERS KING */
        //check for enemy units attacks
        for (int i = 0; i < notMovingPlayer.Units.Count; i++)
        {
            IGamePiece EnemyPiece = notMovingPlayer.Units[i].GetComponent<IGamePiece>();
            if (EnemyPiece == ActualOccupier)
                continue;
            EnemyPiece.SetPossibleMoves();
            if (newPiece is King && EnemyPiece.GetPossibleAttacks().Contains(move))
            {
                badMove = true;
            }
            else if (EnemyPiece.GetPossibleAttacks().Contains(movingPlayer.PlayersKing.GetCurrentTile()))
            {
                badMove = true;
            }
        }
        /* END CHECK IF MOVE ENDANGERS KING */
        /* UNDO MOVE SWAP */
        //newPieceActualTile.Occupier = newPiece;
        game.Board.SetOccupier(newPiece, newPieceActualTile.SquareNumber);
        newPiece.SetCurrentTile(newPieceActualTile);
        game.Board.SetOccupier(ActualOccupier, moveSquare);
        if (hadOccupier)
        {
            ActualOccupier.SetCurrentTile(move);
        }

        for (int i = 0; i < notMovingPlayer.Units.Count; i++)
        {
            IGamePiece enemyPiece = notMovingPlayer.Units[i].GetComponent<IGamePiece>();
            enemyPiece.SetPossibleMoves();
        }
        /* END UNDO MOVE SWAP */
        //if(badMove)
        //    Debug.Log("King would be in check. Move must protect king.");
        return badMove;
    }
    /*
     
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


*/





    /*
     * private readonly ChessGame game;

    public ChessRules(ChessGame game);

    public bool IsCheck();

    public bool IsCheckmate();

    public bool IsStalemate();

    public bool CanKingEscape();

    public bool CanDestroyCheckingUnit();

    public bool CanPathBlock();

    public bool IsLegalMove(
        IGamePiece piece,
        BoardSquare destination);

    public bool MoveLeavesKingInCheck(
        IGamePiece piece,
        BoardSquare destination);

    public bool IsKingInCheck(
        Player defendingPlayer);

    public List<BoardSquare> GetThreatenedSquares(
        Player attackingPlayer);

    public List<IGamePiece> GetCheckingPieces(
        Player attackingPlayer,
        Player defendingPlayer);

    public bool HasAnyLegalMove(
        Player player);

    public bool IsSquareUnderAttack(
        BoardSquare square,
        Player attackingPlayer);

    public bool CanCastle(
        IGamePiece king,
        IGamePiece rook);

    public bool CanEnPassant(
        IGamePiece pawn,
        BoardSquare targetSquare);

    public bool CanPromote(
        IGamePiece pawn);

    public void PromotePawn(
        IGamePiece pawn,
        PieceType promotionType);
     */
}
