using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour, IGamePiece
{
    [SerializeField] private BoardSquare currentTile;
    private List<BoardSquare> possibleMoves;
    private List<BoardSquare> possibleAttacks;
    [SerializeField] private Color Color;
    
    //NW = NORTH WEST, so knight goes north 2 then west 1.
    readonly public static int NW = 17;
    readonly public static int NE = 15;
    readonly public static int SW = -15;
    readonly public static int SE = -17;
    readonly public static int EN = 6;
    readonly public static int ES = -10;
    readonly public static int WN = 10;
    readonly public static int WS = -6;

    private BoardPlane board;
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

    //void Awake()
    //{
    //    possibleMoves = new List<BoardSquare>();//move to other constructor
    //    possibleAttacks = new List<BoardSquare>();
    //    BoardSquare tile = GameManager.Instance.BoardPlane.GetTileBeneath(gameObject);
    //    SetCurrentTile(tile);
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
        int squareNum = currentTile.SquareNumber + NW;
        Move(moves, attacks, squareNum, NW);
        squareNum = currentTile.SquareNumber + NE;
        Move(moves, attacks, squareNum, NE);
        squareNum = currentTile.SquareNumber + WN;
        Move(moves, attacks, squareNum, WN);
        squareNum = currentTile.SquareNumber + WS;
        Move(moves, attacks, squareNum, WS);
        squareNum = currentTile.SquareNumber + SW;
        Move(moves, attacks, squareNum, SW);
        squareNum = currentTile.SquareNumber + SE;
        Move(moves, attacks, squareNum, SE);
        squareNum = currentTile.SquareNumber + EN;
        Move(moves, attacks, squareNum, EN);
        squareNum = currentTile.SquareNumber + ES;
        Move(moves, attacks, squareNum, ES);
    }


    private void Move(List<BoardSquare> moveList,
                      List<BoardSquare> attackList,
                      int squareNum,
                      int dir)
    {
        if (BoardPlane.InBounds(squareNum) && !NearEdge(squareNum - dir, dir))
        {
            BoardSquare tile = board.GetBoardSquare(squareNum);
            if (board.CanAttack(Color, squareNum))
            {
                attackList.Add(tile);
            }
            else if (board.CanMove(squareNum))
            {
                moveList.Add(tile);
            }
        }
    }

    public bool ValidateMove(BoardSquare move)
    {
        List<BoardSquare> mList = new();
        List<BoardSquare> aList = new();
        GenerateMoves(mList, aList);

        if (mList.Contains(move) || aList.Contains(move))
        {
            return true;
        }
        return false;
    }

    public static bool NearEdge(int squareNum, int dir)
    {
        int index = squareNum - 1;
        int row = (index / 8);// -1;
        int col = (index % 8);// -1;
        if(dir == NW && (row >= 6 || col == 7))
            return true;
        if(dir == NE && (row >= 6 || col == 0))
            return true;
        if(dir == WN && (row == 7 || col >= 6))
            return true;
        if(dir == WS && (row == 0 || col >= 6))
            return true;
        if(dir == EN && (row == 7 || col <= 1))
            return true;
        if(dir == ES && (row == 0 || col <= 1))
            return true;
        if(dir == SW && (row <= 1 || col == 7))
            return true;
        if(dir == SE && (row <= 1 || col == 0))
            return true;
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
