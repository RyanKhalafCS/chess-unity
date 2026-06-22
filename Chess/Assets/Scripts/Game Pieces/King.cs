using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class King : MonoBehaviour, IGamePiece
{
    [SerializeField] private BoardSquare currentTile;
    private List<BoardSquare> possibleMoves;
    private List<BoardSquare> possibleAttacks;
    [SerializeField] private Player.Color Color;
    private bool firstMove = true;

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
    //public King(Player.Color color)
    //{
    //    Color = color;
    //    possibleMoves = new List<BoardSquare>();//move to other constructor
    //    possibleAttacks = new List<BoardSquare>();
    //}

    //void Awake()
    //{
    //    firstMove = true;
    //    possibleMoves = new List<BoardSquare>();
    //    possibleAttacks = new List<BoardSquare>();
    //    SetCurrentTile(board.GetTileBeneath(gameObject));
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
        //BoardSquare square = Movement.CanKingRookSwap(this);
        //possibleMoves.Add(square);
    }


    private void GenerateMoves(List<BoardSquare> moves, List<BoardSquare> attacks)
    {
        Move(moves, attacks, Movement.NEXT_N);
        Move(moves, attacks, Movement.NEXT_S);
        Move(moves, attacks, Movement.NEXT_E);
        Move(moves, attacks, Movement.NEXT_W);
        Move(moves, attacks, Movement.NEXT_NE);
        Move(moves, attacks, Movement.NEXT_NW);
        Move(moves, attacks, Movement.NEXT_SE);
        Move(moves, attacks, Movement.NEXT_SW);
        //for(int i = 0; i < moves.Count; i++) 
        //{
        //    Debug.Log($"{Color} King moves: {moves[i]}");
        //}
    }


    private void Move(List<BoardSquare> moveList, List<BoardSquare> attackList, int dir)
    {
        int squareNum = currentTile.SquareNumber + dir;
        if(BoardPlane.InBounds(squareNum) && !Movement.IsOverEdge(squareNum-dir, dir))
        {
            BoardSquare tile = board.GetBoardSquare(squareNum);
            //if(!Movement.MoveInCheck(this, tile))
            {
                if(rules.CanMove(squareNum))
                {
                    moveList.Add(tile);
                }
                else if(rules.CanAttack(Color, squareNum))
                {
                    attackList.Add(tile);
                }
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

    public void DisableGamePiece()
    {
    }

    public Player.Color GetColor()
    {
        return Color;
    }

    
    public GameObject GetObj()
    {
        return gameObject;
    }
}
