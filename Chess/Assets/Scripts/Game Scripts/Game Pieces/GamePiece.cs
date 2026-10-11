using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Shared behaviour for every chess piece. A concrete piece only has to describe
/// how it moves by implementing <see cref="GenerateMoves"/>.
/// </summary>
public abstract class GamePiece : MonoBehaviour, IGamePiece
{
    [FormerlySerializedAs("Color")] [SerializeField] private Color _color;
    [FormerlySerializedAs("currentTile")] [SerializeField] private BoardSquare _currentTile;
    private List<BoardSquare> _possibleMoves;
    private List<BoardSquare> _possibleAttacks;
    private BoardPlane _board;
    private bool _initialized = false;
    private int _id;
    private static int _nextId = 1;

    /// <summary>Unique identifier for this piece, assigned the first time it is needed.</summary>
    public int Id
    {
        get
        {
            if (_id == 0)
                _id = _nextId++;
            return _id;
        }
    }

    public bool Initialized => _initialized;
    protected BoardPlane Board => _board;
    protected BoardSquare CurrentTile => _currentTile;
    protected Color PieceColor => _color;


    public void Initialize(BoardPlane board)
    {
        if (_initialized)
            return;
        _ = Id; //assign ids in initialization order
        OnInitialize();
        _board = board;
        _possibleMoves = new List<BoardSquare>();
        _possibleAttacks = new List<BoardSquare>();

        BoardSquare tile = board.GetTileBeneath(gameObject);
        SetCurrentTile(tile);
        //In a real game each piece claims the square it starts on.
        //Test games place their pieces manually.
        if (!board.Testing && tile != null)
        {
            board.SetOccupier(this, tile.SquareNumber);
        }
        _initialized = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetIds()
    {
        _nextId = 1;
    }

    /// <summary>Hook for piece-specific setup (e.g. resetting FirstMove).</summary>
    protected virtual void OnInitialize() { }

    /// <summary>Fills the given lists with every square this piece could move to / attack.</summary>
    protected abstract void GenerateMoves(List<BoardSquare> moves, List<BoardSquare> attacks);


    public void SetPossibleMoves()
    {
        _possibleMoves.Clear();
        _possibleAttacks.Clear();
        GenerateMoves(_possibleMoves, _possibleAttacks);
    }

    public List<BoardSquare> GetPossibleMoves()
    {
        return _possibleMoves;
    }

    public List<BoardSquare> GetPossibleAttacks()
    {
        return _possibleAttacks;
    }

    public bool ValidateMove(BoardSquare move)
    {
        List<BoardSquare> moves = new();
        List<BoardSquare> attacks = new();
        GenerateMoves(moves, attacks);
        return moves.Contains(move) || attacks.Contains(move);
    }


    /// <summary>
    /// Adds the square to <paramref name="moves"/> if it is empty, or to
    /// <paramref name="attacks"/> if it holds an enemy piece.
    /// Returns true only when the square was empty, i.e. a sliding piece may keep going.
    /// </summary>
    protected bool TryAddSquare(List<BoardSquare> moves, List<BoardSquare> attacks, int squareNum)
    {
        BoardSquare tile = _board.GetBoardSquare(squareNum);
        if (_board.CanMove(squareNum))
        {
            moves.Add(tile);
            return true;
        }
        if (_board.CanAttack(_color, squareNum))
        {
            attacks.Add(tile);
        }
        return false;
    }


    public BoardSquare GetCurrentTile()
    {
        return _currentTile;
    }

#nullable enable
    public void SetCurrentTile(BoardSquare? obj)
    {
        _currentTile = obj;
    }
#nullable restore

    public Color GetColor()
    {
        return _color;
    }

    public GameObject GetObj()
    {
        return gameObject;
    }

    public void DisableGamePiece()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnMoved() { }

    public void TestInit(Color color)
    {
        if (_board.Testing)
        {
            _color = color;
        }
    }
}
