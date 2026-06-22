using UnityEngine;

/*
 * TODO notes:
 * Asterisk + tilda (* ~) means completed
 * 
 * Why tf is king not included in player.Units list??? Change it. CTRL + F "units" and change...
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


public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private BoardPlane board;
    public BoardPlane Board => board;
    public  Game Game { get; private set; }
    [SerializeField] private ColorSelectPopup popup;


    protected override void Awake()
    {
        base.Awake();
        //game = new(board);
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        popup.Show("Will you be white or black?", OnColorChosen);
    }

    //create new game
    private void OnColorChosen(bool colorBool)
    {
        Player.Color color;
        if (colorBool)
        {
            color = Player.Color.White;
        }
        else
        {
            color = Player.Color.Black;
        }
        Game = new(Board, color);
    }


    /// <summary>
    /// Move Validation
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="moveToTile"></param>
    public void RequestMove(GameObject obj, BoardSquare moveToTile)//SHOULD NOT BE PASSING AROUND ENTIRE OBJECTS TOO MUCH.
    {
        if (obj == null || moveToTile == null)
        {
            return;
        }
        Game.TryMove(obj, moveToTile);
    }

}