using System.Collections.Generic;
using UnityEngine;

public class GameRules
{
    private readonly Game game;

    public GameRules(Game game)
    {
        this.game = game;
    }



    //bounds checking needed? Dont think so, either SelectPiece checks or gamePiece.move
    public bool ValidMove(IGamePiece gPiece, BoardSquare move)
    {
        if (move == null)
            return false;
        if (move == gPiece.GetCurrentTile())
            return false;
        IGamePiece occupier = game.Board.GetOccupier(move.SquareNumber);
        //Debug.Log("VALIDMOVE 1= " + occupier);
        // Reject a move onto a same-color piece up front. This also keeps MoveSim from
        // simulating a "capture" of a friendly piece (which pollutes the Units rosters).
        if (occupier != null && occupier.GetColor() == gPiece.GetColor())
            return false;
        gPiece.SetPossibleMoves();
        if (game.MoveSim.MoveInCheck(gPiece, move, game))
        {
            Debug.Log("King would be in check. Move must protect king.");
            return false;
        }
        occupier = game.Board.GetOccupier(move.SquareNumber);
        //Debug.Log("VALIDMOVE 2= " + occupier);
        if (gPiece.GetPossibleMoves().Contains(move))
        {
            return true;
        }
        occupier = game.Board.GetOccupier(move.SquareNumber);
        //Debug.Log("VALIDMOVE 3= " + occupier);
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
    public bool IsInCheck(Player playerMoving, Player playerNotMoving)
    {
        var currPlayerUnitsChecking = playerMoving.UnitsCheckingEnemyKing;
        currPlayerUnitsChecking.Clear();
        for (int i = 0; i < playerMoving.Units.Count; i++)
        {
            if (playerMoving.Units[i].GetComponent<IGamePiece>().GetPossibleAttacks().Contains(playerNotMoving.PlayersKing.GetCurrentTile()))
            {
                currPlayerUnitsChecking.Add(playerMoving.Units[i].GetComponent<IGamePiece>());
            }
        }
        if (playerMoving.UnitsCheckingEnemyKing.Count > 0)
        {
            playerNotMoving.SetInCheck(true);
        }
        else
            playerNotMoving.SetInCheck(false);
        return playerNotMoving.Check;
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
        //_ = game.CurrentPlayer;
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

        game.UpdateMoveLists();
        IsInCheck(game.CurrentPlayer, game.OtherPlayer);
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
        BoardSquare tileToAttack = game.CurrentPlayer.UnitsCheckingEnemyKing[0].GetCurrentTile();
        for (int i = 0; i < game.OtherPlayer.Units.Count; i++)
        {
            if (!game.OtherPlayer.Units[i].TryGetComponent<IGamePiece>(out IGamePiece enemyGamePiece))
            {
                continue;
            }
            if (!enemyGamePiece.GetPossibleAttacks().Contains(tileToAttack))
            {
                continue;
            }
            if (ValidMove(enemyGamePiece, tileToAttack))
            {
                return true;
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
                if (ValidMove(unitDefending, tile))
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
