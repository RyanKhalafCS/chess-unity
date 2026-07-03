using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
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
    public MoveSim MoveSim { get; }
    public bool TestGame { get; }


    public Game(BoardPlane board, bool testGame)
    {
        CurrentState = GameState.Initializing;
        this.Board = board;
        this.Board.Initialize(true);
        MoveSim = new MoveSim();
        Rules = new GameRules(this);
        this.TestGame = testGame;
        StartGame(Color.White);
    }

    public Game(BoardPlane board, Color color)
    {
        CurrentState = GameState.Initializing;
        this.Board = board;
        this.Board.Initialize(false);
        MoveSim = new MoveSim();
        Rules = new GameRules(this);
        this.TestGame = false;
        StartGame(color);
    }

    private void StartGame(Color color)
    {
        InitGame(color);//move down to line before enable?
        InitializePieces(CurrentPlayer, Board);
        InitializePieces(OtherPlayer, Board);
        // Ensure only the current player can select pieces at start
        DisableUnits(OtherPlayer);
        EnableUnits(CurrentPlayer);
    }
    private void InitGame(Color color)
    {
        PlayerManager playerManager = new(color, TestGame);

        if (playerManager.Players[0].PlayerColor == Color.White)
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

    private void InitializePieces(Player player, BoardPlane board)
    {
        for(int i = 0; i < player.Units.Count; i++)
        {
            if(!player.Units[i].TryGetComponent<IGamePiece>(out IGamePiece piece))
            {
                //EXCEPTION
            }
            piece.Initialize(board);
        }
    }

    public void TryMove(GameObject obj, BoardSquare moveToTile)
    {
        //if obj is not one of CurrentPlayer's units and it's not their king, bad obj.
        if (!CurrentPlayer.Units.Contains(obj))
        {
            return;
        }
        if (obj.TryGetComponent<IGamePiece>(out IGamePiece piece))
        {
            if (Rules.ValidMove(piece, moveToTile) && piece.ValidateMove(moveToTile))
            {
                int tileNum = moveToTile.SquareNumber;
                if (Board.GetOccupier(tileNum) != null)
                {
                    IGamePiece temp = (IGamePiece)Board.GetOccupier(tileNum);
                    DestroyUnit(temp.GetObj());
                }
                Board.MovePiece(piece, tileNum);
                //should make class for moving visual GameObjects
                obj.transform.position = moveToTile.transform.position;

                piece.OnMoved();    //Sets first move to false

                ProcessMove();
            }
        }
    }
    


    internal void ProcessMove()
    {
        DisableUnits(CurrentPlayer);
        UpdateMoveLists();
        Rules.IsInCheck(CurrentPlayer, OtherPlayer);
        NextGameState();
        NextCurrentPlayer();
        if ((CurrentState == GameState.WhiteTurn && CurrentPlayer.PlayerColor == Color.White)
            || (CurrentState == GameState.BlackTurn && CurrentPlayer.PlayerColor == Color.Black))
        {
            EnableUnits(CurrentPlayer);
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


    public void UpdateMoveLists()
    {
        for (int i = 0; i < CurrentPlayer.Units.Count; i++)
        {
            CurrentPlayer.Units[i].GetComponent<IGamePiece>().SetPossibleMoves();
        }
        for (int i = 0; i < OtherPlayer.Units.Count; i++)
        {
            OtherPlayer.Units[i].GetComponent<IGamePiece>().SetPossibleMoves();
        }
    }

    //RECALCULATE UNIT MOVES/ATTACKS?
    private void DestroyUnit(GameObject obj)
    {
        OtherPlayer.RemoveUnit(obj);//maybe remove unit should return unit
        IGamePiece piece = obj.GetComponent<IGamePiece>();
        if (OtherPlayer.UnitsCheckingEnemyKing.Contains(piece))
            OtherPlayer.UnitsCheckingEnemyKing.Remove(piece);
        Board.SetOccupier(null, piece.GetCurrentTile().SquareNumber);
        piece.SetCurrentTile(null);
        //obj.SetActive(false);
        Object.Destroy(obj);
    }


    //private void ToggleUnits()
    //{//toggle units enabled based off of current state?
    //    //Disable current player's SelectPiece component if present.
    //     for (int i = 0; i < CurrentPlayer.Units.Count; i++)
    //     {
    //         GameObject unit = CurrentPlayer.Units[i];
    //         if (unit == null) continue;
    //         if (unit.TryGetComponent<SelectPiece>(out SelectPiece sp))
    //         {
    //            sp.enabled = !sp.enabled;
    //            sp.OnDisable();
    //         }
    //         else
    //         {
    //             Debug.Log($"i = {i}, Unit '{unit.name}' has no SelectPiece component.");
    //         }
    //     }
    //}
    private void DisableUnits(Player player)
    {
        // Disable current player's SelectPiece component if present.
        for (int i = 0; i < player.Units.Count; i++)
        {
            GameObject unit = player.Units[i];
            if (unit == null) continue;
            if (unit.TryGetComponent<SelectPiece>(out SelectPiece sp))
            {
                sp.enabled = false;
                sp.OnDisable();
            }
            else
            {
                Debug.Log($"i = {i}, Unit '{unit.name}' has no SelectPiece component.");
            }
        }
    }
    private void EnableUnits(Player player)
    {
        // Enable new current player's SelectPiece component if present.
        for (int i = 0; i < player.Units.Count; i++)
        {
            GameObject unit = player.Units[i];
            if (unit == null) continue;
            if (unit.TryGetComponent<SelectPiece>(out SelectPiece sp))
            {
                sp.enabled = true;
            }
            else
            {
                Debug.Log($"Unit '{unit.name}' has no SelectPiece component to enable.");
            }
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
        // Disable selection for both players when the game ends
        DisableUnits(CurrentPlayer);
        DisableUnits(OtherPlayer);
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
