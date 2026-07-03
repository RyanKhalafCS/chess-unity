using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


//super boss, transformation, full power
//uncontrollable pow, revenge, planetary destr
public class Queen : MonoBehaviour, IGamePiece
{
    [SerializeField] private BoardSquare currentTile;//private?
    private List<BoardSquare> possibleMoves;
    private List<BoardSquare> possibleAttacks;
    [SerializeField] private Color Color;

    private BoardPlane board;
    public bool Initialized => initialized;
    private bool initialized = false;

    public void Initialize(BoardPlane board)
    {
        if (initialized)
            return;
        this.board = board;
        possibleMoves = new List<BoardSquare>();
        possibleAttacks = new List<BoardSquare>();
        SetCurrentTile(board.GetTileBeneath(gameObject));
        initialized = true;
    }


    //private void Awake()
    //{
    //    possibleMoves = new List<BoardSquare>();//move to other constructor
    //    possibleAttacks = new List<BoardSquare>();
    //    SetCurrentTile(board.GetTileBeneath(gameObject));
    //}

    public void DisableGamePiece()
    {
        gameObject.SetActive(false);
    }

    public BoardSquare GetCurrentTile()
    {
        return currentTile;
    }

    public List<BoardSquare> GetPossibleAttacks()
    {
        return possibleAttacks;
    }

    public List<BoardSquare> GetPossibleMoves()
    {
        return possibleMoves;
    }


    #nullable enable
    public void SetCurrentTile(BoardSquare? obj)
    {
        currentTile = obj;
    }

    public void SetPossibleMoves()
    {
        possibleMoves.Clear();
        possibleAttacks.Clear();
        GenerateMoves(possibleMoves, possibleAttacks);
    }


    private void GenerateMoves(List<BoardSquare> moves, List<BoardSquare> attacks)
    {
        Move(moves, attacks, Movement.NEXT_N);
        Move(moves, attacks, Movement.NEXT_S);
        Move(moves, attacks, Movement.NEXT_E);
        Move(moves, attacks, Movement.NEXT_W);
        Move(moves, attacks, Movement.NEXT_NW);
        Move(moves, attacks, Movement.NEXT_NE);
        Move(moves, attacks, Movement.NEXT_SW);
        Move(moves, attacks, Movement.NEXT_SE);
    }

    private void Move(List<BoardSquare> moveList,
                      List<BoardSquare> attackList,
                      int dir)
    {  
        bool endpoint = false;
        //Debug.Log("HERE: " + StackFrame(1).GetMethod().Name)
        int squareNum = GetCurrentTile().SquareNumber + dir;
        while (BoardPlane.InBounds(squareNum) && !Movement.IsOverEdge(squareNum - dir, dir))
        {
            CanMoveTo(moveList, attackList, squareNum, ref endpoint);
            if(endpoint)
                break;
            squareNum += dir;
        }
    }


    private void CanMoveTo(List<BoardSquare> moveList,
                          List<BoardSquare> attackList,
                          int squareNum,
                          ref bool endpoint)
    {
        BoardSquare tile = board.GetBoardSquare(squareNum);
        if (board.CanMove(squareNum))
        {
            moveList.Add(tile);
        }
        else if (board.CanAttack(Color, squareNum))
        {
            attackList.Add(tile);
            endpoint = true;
        }
        else
        {
            endpoint = true; // blocked by own piece or invalid
        }
    }


    public bool ValidateMove(BoardSquare move)
    {
        List<BoardSquare> mList = new();
        List<BoardSquare> aList = new();
        GenerateMoves(mList, aList);

        if(mList.Contains(move) || aList.Contains(move))
        {
            return true;
        }
        return false;
    }

    public Color GetColor()
    {
        return Color;
    }

    
    public GameObject GetObj()
    {
        return gameObject;
    }


    public virtual void OnMoved()
    {

    }
    public void TestInit(Color color)
    {
        if (board.Testing)
        {
            this.Color = color;
        }
    }
}
