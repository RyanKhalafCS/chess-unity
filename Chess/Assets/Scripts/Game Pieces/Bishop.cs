using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering.Universal.Internal;

public class Bishop : MonoBehaviour, IGamePiece
{
    //public GameObject prefab;
    [SerializeField] private Player.Color Color;
    [SerializeField] private BoardSquare currentTile;
    private List<BoardSquare> possibleMoves;
    private List<BoardSquare> possibleAttacks;
    private GameRules rules;
    private BoardPlane board;
    private bool initialized = false;

    public void Initialize(GameRules rules, BoardPlane board)
    {
        if (initialized)
            return;
        this.rules = rules;
        this.board = board;
        possibleMoves = new List<BoardSquare>();
        possibleAttacks = new List<BoardSquare>();
        SetCurrentTile(board.GetTileBeneath(gameObject));
        initialized = true;
    }
    /* public void CreatePrefab(Vector3 onTile)
    {
        GameObject pawn = Instantiate(prefab, onTile, Quaternion.identity);
    } */

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

    public Player.Color GetColor()
    {
        return Color;
    }
    
    public GameObject GetObj()
    {
        return gameObject;
    }
}
