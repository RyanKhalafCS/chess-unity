using System.Collections.Generic;
using UnityEngine;

public class Bishop : MonoBehaviour, IGamePiece
{
    private const string BLACK_BISHOP_PF = "Prefabs/White Bishop";
    private const string WHITE_BISHOP_PF = "Prefabs/Black Bishop";
    //public GameObject prefab;
    [SerializeField] private Color Color;
    [SerializeField] private BoardSquare currentTile;
    private List<BoardSquare> possibleMoves;
    private List<BoardSquare> possibleAttacks;
    private BoardPlane board;
    private bool initialized = false;

    public Bishop(Color color, BoardSquare tile)
    {
        //string prefab = (color == Player.Color.White)
        //    ? WHITE_BISHOP_PF
        //    : BLACK_BISHOP_PF;

        //GameObject bishop = Instantiate(Resources.Load<GameObject>(prefab));
    }


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

    public List<BoardSquare> GetPossibleMoves()
    {
        return possibleMoves;
    }
    public List<BoardSquare> GetPossibleAttacks()
    {
        return possibleAttacks;
    }

    public void SetPossibleMoves()
    {
        possibleAttacks.Clear();
        possibleMoves.Clear();
        GenerateMoves(possibleMoves, possibleAttacks);
    }

    private void GenerateMoves(List<BoardSquare> moves, List<BoardSquare> attacks)
    {

        Move(moves, attacks, Movement.NEXT_NW);
        Move(moves, attacks, Movement.NEXT_NE);
        Move(moves, attacks, Movement.NEXT_SW);
        Move(moves, attacks, Movement.NEXT_SE);
    }

    private void Move(List<BoardSquare> moveList, List<BoardSquare> attackList, int dir)
    {
        bool endpoint = false;
        int squareNum = GetCurrentTile().SquareNumber + dir;
        while(BoardPlane.InBounds(squareNum) && !Movement.IsOverEdge(squareNum-dir, dir))
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

        if (mList.Contains(move) || aList.Contains(move))
        {
            return true;
        }
        return false;
    }


    public BoardSquare GetCurrentTile()
    {
        return currentTile;
    }
    
    #nullable enable
    public void SetCurrentTile(BoardSquare? obj)
    {
        currentTile = obj;
    }

    

    public void DisableGamePiece()//probly delete these
    {
        gameObject.SetActive(false);
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
        if(board.Testing)
        {
            this.Color = color;
        }
    }
}
