using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pawn : MonoBehaviour, IGamePiece
{
    public bool FirstMove { get; set; }
    [SerializeField] private Color Color;
    [SerializeField] private BoardSquare currentTile;
    private List<BoardSquare> possibleMoves;
    private List<BoardSquare> possibleAttacks;
    private BoardPlane board;
    private bool initialized = false;

    public void Initialize(BoardPlane board)
    {
        if (initialized)
            return;
        FirstMove = true;
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
        possibleMoves.Clear();
        possibleAttacks.Clear();

        GenerateMoves(possibleMoves, possibleAttacks);
        
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


    private void GenerateMoves(List<BoardSquare> moves, List<BoardSquare> attacks)
    {
        int colorMultiplier = Movement.GetColorMultiplier(Color);

        //Attack NW
        int dir = Movement.NEXT_NW * colorMultiplier;
        int squareNum = currentTile.SquareNumber + dir;
        CanAttack(moves, attacks, squareNum, dir);

        //Attack NE
        dir = Movement.NEXT_NE * colorMultiplier;
        squareNum = currentTile.SquareNumber + dir;
        CanAttack(moves, attacks, squareNum, dir);

        //SET MOVE TILES
        dir = Movement.NEXT_N * colorMultiplier;
        squareNum = currentTile.SquareNumber + dir;
        CanMoveTo(moves, squareNum, dir);
    }

    private void CanAttack(List<BoardSquare> moveList,
                          List<BoardSquare> attackList,
                          int squareNum,
                          int dir)
    {
        if (BoardPlane.InBounds(squareNum)
            && !Movement.IsOverEdge(squareNum - dir, dir)
            && board.CanAttack(Color, squareNum))
        {
            BoardSquare tile = board.GetBoardSquare(squareNum);
            attackList.Add(tile);
        }
    }

    private void CanMoveTo(List<BoardSquare> moveList, int squareNum, int dir)
    {

        if (BoardPlane.InBounds(squareNum) && board.CanMove(squareNum))
        {
            BoardSquare tile = board.GetBoardSquare(squareNum);
            moveList.Add(tile);

            //Check second N tile
            squareNum += dir;
            if (BoardPlane.InBounds(squareNum) && FirstMove && board.CanMove(squareNum))//unnecessary inbounds check
            {
                tile = board.GetBoardSquare(squareNum);
                moveList.Add(tile);
            }
        }
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
    public void DisableGamePiece()
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

    public void OnMoved()
    {
        FirstMove = false;
    }
    public void TestInit(Color color)
    {
        if (board.Testing)
        {
            this.Color = color;
        }
    }
}
