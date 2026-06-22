using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Rook : MonoBehaviour, IGamePiece
{
    [SerializeField] private BoardSquare currentTile;
    private List<BoardSquare> possibleMoves;
    private List<BoardSquare> possibleAttacks;
    [SerializeField] private Player.Color Color;
    private bool firstMove = true; //CAN ROOK SWAP~~ADD!!!!!!!!!!!

    private GameRules rules;
    private BoardPlane board;
    private bool initialized = false;

    public void Initialize(GameRules rules, BoardPlane board)
    {
        if (initialized)
            return;
        firstMove = true;
        this.rules = rules;
        this.board = board;
        possibleMoves = new List<BoardSquare>();
        possibleAttacks = new List<BoardSquare>();
        SetCurrentTile(board.GetTileBeneath(gameObject));
        initialized = true;
    }
    //private Rook()
    //{
    //}
    //public Rook(Player.Color color)
    //{
    //    Color = color;
    //    possibleMoves = new List<BoardSquare>();//move to other constructor
    //    possibleAttacks = new List<BoardSquare>();
    //    firstMove = true;
    //}

    //void Awake()
    //{
    //    firstMove = true;
    //    possibleMoves = new List<BoardSquare>();
    //    possibleAttacks = new List<BoardSquare>();
    //    SetCurrentTile(GameManager.Instance.BoardPlane.GetTileBeneath(gameObject));
    //}

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
    }
    private void Move(List<BoardSquare> moveList, List<BoardSquare> attackList, int dir)
    {
        bool endpoint = false;
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
        if (rules.CanMove(squareNum))
        {
            moveList.Add(tile);
        }
        else if (rules.CanAttack(Color, squareNum))
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
            firstMove = false;
            return true;
        }
        return false;
    }


    //private void RookSwap()
    //{
    //    if(firstMove)
    //    {
    //        int squareNum1 = currentTile.squareNumber + 1;
    //        int squareNum2 = currentTile.squareNumber + 2;
    //        int squareNum3 = currentTile.squareNumber + 3;
    //        BoardSquare tile1 = BoardPlane.GetBoardSquare(squareNum1);
    //        BoardSquare tile2 = BoardPlane.GetBoardSquare(squareNum2);
    //        BoardSquare tile3 = BoardPlane.GetBoardSquare(squareNum3);
    //        King king = (King)tile3.Occupier;
    //        if(king.firstMove)
    //        {
    //            if(tile1.Occupier == null
    //                && tile2.Occupier == null)
    //            {
    //                //set kings tile occupier to null, set rook tile occupier to null. 
    //                GetCurrentTile().Occupier = null;
    //                king.GetCurrentTile().Occupier = null;
    //                SetCurrentTile(tile2);
    //                king.SetCurrentTile(tile1);
    //                GetCurrentTile().Occupier = this;
    //                king.GetCurrentTile().Occupier = king;
    //            }
    //        }
    //    }
    //}


    


    public Player.Color GetColor()
    {
        return Color;
    }
    
    public void DisableGamePiece()
    {
        gameObject.SetActive(false);
    }
    
    public GameObject GetObj()
    {
        return gameObject;
    }
}
