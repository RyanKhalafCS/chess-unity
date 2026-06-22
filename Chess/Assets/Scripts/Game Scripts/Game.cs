using UnityEngine;

public class Game
{
    public enum GameState
    {
        Initializing,
        WhiteTurn,
        BlackTurn,//add PostTurn? YES. PostTurn should be the only time that game can be truly modified.
        //Special game states
        Check,
        Checkmate,
        Paused,
        Stalemate,
        //End game states
        Draw,
        Resigned,
        GameOver,
        AwaitingRematch
    }

    public GameState CurrentState { get; private set; }
    public Player WhitePlayer;
    public Player BlackPlayer;
    public Player CurrentPlayer { get; private set; }
    public Player OtherPlayer { get; private set; }
    public BoardPlane Board { get; }
    public GameRules Rules { get; }


    public Game(BoardPlane board, Player.Color color)
    {
        Board = board;

        //Player whitePlayer = new Player(PlayerColor.White);
        //Player blackPlayer = new Player(PlayerColor.Black);

        //CurrentPlayer = whitePlayer;
        //OtherPlayer = blackPlayer;

        CurrentState = GameState.Initializing;
        InitGame(color);
        Rules = new GameRules(this);
        InitializePieces(CurrentPlayer, Board, Rules);
        InitializePieces(OtherPlayer, Board, Rules);
        EnableUnits();
    }


    private void InitGame(Player.Color color)
    {
        PlayerManager playerManager = new(color);

        if (playerManager.Players[0].PlayerColor == Player.Color.White)
        {
            CurrentPlayer = playerManager.Players[0];
            OtherPlayer = playerManager.Players[1];
        }
        else
        {
            CurrentPlayer = playerManager.Players[1];
            OtherPlayer = playerManager.Players[0];
        }
        CurrentState = GameState.WhiteTurn;
        Debug.Log("Current Player = " + CurrentPlayer
            + "\nCurrent Player Color = " + CurrentPlayer.PlayerColor);
    }

    private void InitializePieces(Player player, BoardPlane board, GameRules rules)
    {
        for(int i = 0; i < player.Units.Count; i++)
        {
            if(!player.Units[i].TryGetComponent<IGamePiece>(out IGamePiece piece))
            {
                //EXCEPTION
            }
            piece.Initialize(rules, board);
        }
    }

    public void TryMove(GameObject obj, BoardSquare moveToTile)
    {
        GameObject king = CurrentPlayer.PlayersKing.GetObj();
        //if obj is not one of CurrentPlayer's units and it's not their king, bad obj.
        if (!CurrentPlayer.Units.Contains(obj))
        {
            return;
        }
        if (obj.TryGetComponent<IGamePiece>(out IGamePiece piece))
        {
            if (piece.ValidateMove(moveToTile) && Rules.ValidMove(piece, moveToTile))
            {
                int tileNum = moveToTile.SquareNumber;
                if (Board.GetOccupier(tileNum) != null)
                {
                    DestroyUnit(Board.GetOccupier(tileNum).GetObj());
                }
                Board.MovePiece(piece, tileNum);
                //should make class for moving visual GameObjects
                obj.transform.position = moveToTile.transform.position;
                ProcessMove();
            }
        }
    }
    


    internal void ProcessMove()
    {
        DisableUnits();
        UpdateMoveLists();
        Rules.IsInCheck();
        NextGameState();
        NextCurrentPlayer();
        if ((CurrentState == GameState.WhiteTurn && CurrentPlayer.PlayerColor == Player.Color.White)
            || (CurrentState == GameState.BlackTurn && CurrentPlayer.PlayerColor == Player.Color.Black))
        {
            EnableUnits();
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~NEXT TURN~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.Log("GameState = " + CurrentState + "Current Player = " + CurrentPlayer);
        }
        else if (CurrentState == GameState.Checkmate)
        {
            OnCheckmate();
        }
        else
        {
            Debug.LogError("ERROR! Game state problem!");
            OnGameOver();
        }
    }

    private void NextCurrentPlayer()
    {
        (OtherPlayer, CurrentPlayer) = (CurrentPlayer, OtherPlayer);
    }


    private void NextGameState()
    {
        //Initializing > WhiteTurn > BlackTurn > (repeat turns) > Checkmate, Stalemate, Draw, Resign, Game Over > Rematch
        //need to check which units are threatening the king. If they can be attacked, then not checkmate.
        bool checkmate = Rules.IsCheckmate();
        if (checkmate)
        {
            CurrentState = GameState.Checkmate;
        }
        else if (CurrentState == GameState.WhiteTurn)
        {
            CurrentState = GameState.BlackTurn;
        }
        else if (CurrentState == GameState.BlackTurn)
        {
            CurrentState = GameState.WhiteTurn;
        }
        else
            Debug.Log("ERROR IN NextGameState()");
    }


    private void UpdateMoveLists()
    {
        for (int j = 0; j < CurrentPlayer.Units.Count; j++)
        {
            CurrentPlayer.Units[j].GetComponent<IGamePiece>().SetPossibleMoves();
        }
        for (int j = 0; j < OtherPlayer.Units.Count; j++)
        {
            OtherPlayer.Units[j].GetComponent<IGamePiece>().SetPossibleMoves();
        }
    }

    //RECALCULATE UNIT MOVES/ATTACKS?
    public void DestroyUnit(GameObject obj)
    {
        OtherPlayer.RemoveUnit(obj);
        IGamePiece piece = obj.GetComponent<IGamePiece>();
        if (OtherPlayer.UnitsCheckingEnemyKing.Contains(piece))
            OtherPlayer.UnitsCheckingEnemyKing.Remove(piece);
        Board.SetOccupier(null, piece.GetCurrentTile().SquareNumber);//obj.GetComponent<IGamePiece>().GetCurrentTile().Occupier = null;
        piece.SetCurrentTile(null);
        obj.SetActive(false);
    }



    private void DisableUnits()
    {
        //Disable OtherPlayers's actions.
        for (int i = 0; i < CurrentPlayer.Units.Count; i++)
        {
            Object.Destroy(CurrentPlayer.Units[i].GetComponent<SelectPiece>());
        }
    }
    private void EnableUnits()
    {
        //Enable new CurrentPlayers actions.
        for (int i = 0; i < CurrentPlayer.Units.Count; i++)
        {
            CurrentPlayer.Units[i].AddComponent<SelectPiece>();
        }
    }


    private void OnCheck()
    {
        if (CurrentState == GameState.WhiteTurn || CurrentState == GameState.BlackTurn)
        {
            //CurrentState = GameState.Check;  // Notify that the King is in check
        }
    }

    private void OnCheckmate()
    {
        DisableUnits();
        Debug.Log("CHECKMATE!\n" + CurrentPlayer.PlayerColor + " Wins!");
        //Open popup prompt to either exit or restart.
    }

    private void OnStalemate()
    {
        CurrentState = GameState.Stalemate;  // End the game with a draw due to stalemate
    }

    private void OnDraw()
    {
        CurrentState = GameState.Draw;  // End the game with a draw
    }

    private void PauseGame()
    {
        CurrentState = GameState.Paused;  // Pause the game
    }

    private void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.WhiteTurn;  // Resume from White's turn
        }
    }

    private void OnResign()
    {
        CurrentState = GameState.Resigned;  // Player has resigned
    }

    private void OnRematch()
    {
        CurrentState = GameState.AwaitingRematch;  // Waiting for players to agree to rematch
    }

    private void OnGameOver()
    {
        CurrentState = GameState.GameOver;
    }

}
