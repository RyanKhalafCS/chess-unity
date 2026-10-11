using UnityEngine;
using UnityEngine.Serialization;

/*
 * TODO notes:
 * Asterisk + tilda (* ~) means completed
 * 
 * maybe use more dictionaries for things like Unit lists. Not really needed though. ID would be useful in this case.
 * MoveInCheck's call to IsInCheck() will still set the player.checked flag.
 * Rethink test flow. Instead of gamePiece.TestInit() to decide colors, find a better, safer way.
 * BUG: after lots of moves, some of the non-turn player's units will still have SelectPiece enabled when they shouldnt, allowing them to attempt a move. Maybe just go back to removeComponent. SetEnabled() is wacky.
 * might be good idea to have gamePiece IDs for some extra functionalities.
 * create a struct to hold vars for PrepareMove() and Unmove()
 * player.Units should be a list of IGamePieces
 * Set access modifiers correctly
 * player.units accessibility need fix. player.units.add(obj) should not be allowed probably. Just dont iterate over that obj in MoveInCheck.
 * GAMEBOARD ORIENTATION IS NOT STANDARD.
 * Add validation to setOccupier and similar?
 * Really shouldve named BoardPlane as BoardPlane instead...
 * IF KINGS ARE ONE SPACE APART AND QUEEN MOVES BETWEEN, CHECKED KING CAN ATTACK QUEEN CURRENTLY. WRONG.
 * ~put move validation and all possibleAttack modifications into game piece defs
 * ensure SetCurrentTile() is safe, boardsquare.occupier too
 * Should make a class for moving visual game objects (obj.transform.position). Not a lot of that happening, but still.
 * In game piece definitions, GetObj() should not be exposed. Must be a better way to call the obj a component is attached to.
 * IN GameManager, use [SerializeField] private Game game; so you can set processMove() to private!!! Other c# options?
 * Should add GamePiece class that implements IGamePiece and is inherited by every game piece, containing common function defs.
 * boardplane.assignPiece uses object.find(), reconsider. At least use findWithTag(). Maybe just init gameboard from code including visuals.
 * NEED MORE NULL CHECKS AND SAFETY. Make each operation with possiblility of failure into a function and add exception handling
 * Change gamePiece.SetCurrentTile(BoardSquare) to (squareNum)
 * gamepiece.disableGamePiece needed?
 * TERMS BOARDSQUARE AND TILE SHOULDNT BE USED INTERCHANGEABLY THROUGHOUT PROGRAM. Also standardize param names, etc.
 * lists possibleMoves and possibleAttacks, as well as methods with BoardSquare params, should really only be getting passed ints, not full objects.
 * ~Anytime a piece moves, use BoardPlane.movePiece(). Even for king-rook swap.
 * ~tile.occupier assignments and piece.setCurrentTile() assignments should always happen at the same time. Create function to maintain this state.
 * ~GamePieces shouldnt be able to move off of game board.
 * ~Bug where gamepiece won't snap back to originalPosition after being dropped. Looks bad but doesn't break game at all.
 * Remember to ~destroy created obj. C# is managed, but still certain situations, such as boardDict in class BoardPlane?
 * comb all the code and put every subroutine into a function. Ensure single purpose rule for classes. Spread out functions to other classes.
 * eventually decide if adding special rules, etc. Add undo?
 * FIX NAMING CONVENTIONS... Make sure access modifiers all make sense.
 * ENSURE PLAYER.CHECK flag is being set correctly
 * FIX NAMING CONVENTIONS.FUNCTIONS MUST START WITH CAPITAL LETTER
 * Internal files not used, can replace internal with public. Can use assembly definition files to control property access and clean up architecture. Also speeds up compilation.

 */


public class GameManager : MonoBehaviour
{
    [FormerlySerializedAs("board")] [SerializeField] private BoardPlane _board;
    public BoardPlane Board => _board;
    public  Game Game { get; private set; }
    [FormerlySerializedAs("gamePieces")] [SerializeField] private GameObject _gamePieces;
    [FormerlySerializedAs("popup")] [SerializeField] private ColorSelectPopup _popup;
    public bool Testing { get; private set; }


    private void Start()
    {
        // Allow tests to opt-in to test-mode before Unity.Start() runs.
        // Tests should set TestRuntime.IsTesting = true before loading the scene.
        //TEST MODE - this mode functions exactly like a normal game, except
        //  that the board is created empty and the Players are created with only a King.
        if (TestRuntime.IsTesting)
        {
            this.Testing = true;
            StartTestGame();
        }
        else
        {
            this.Testing = false;
            StartGame();
        }
    }


    private void StartTestGame()
    {
        Debug.Log("////////////////TEST MODE////////////////");
        Game = new(Board);
    }

    private void StartGame()
    {
        _popup.Show("Will you be white or black?", OnColorChosen);
    }

    //create new game
    private void OnColorChosen(bool colorBool)
    {
        Color color;
        color = (colorBool) ? Color.White : Color.Black;
        //gamePieces.SetActive(true);
        Game = new(Board, color);
    }


    /// <summary>
    /// Move Validation
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="moveToTile"></param>
    public bool RequestMove(GameObject obj, BoardSquare moveToTile)//SHOULD NOT BE PASSING AROUND ENTIRE OBJECTS TOO MUCH.
    {
        if (obj == null || moveToTile == null)
        {
            return false;
        }
        return Game.TryMove(obj, moveToTile);
    }

}