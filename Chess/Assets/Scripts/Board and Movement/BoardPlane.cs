using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class BoardPlane : MonoBehaviour
{
    bool initialized = false;//track if Setup() was already called once?
    public const int SIZE = 64;
    public const int SIDE_LEN = 8;
    private const string TILE_PLANE_LAYER = "Tile Plane";
    private readonly BoardSquare[,] board = new BoardSquare[8, 8];
    [SerializeField] private GameObject[] tileLayer = new GameObject[64];
    private readonly Dictionary<int, BoardSquare> boardDict = new();//<squareNum, Square>
    private readonly IGamePiece[] occupier = new IGamePiece[64];

    void Awake()
    {
    }
    void Start()
    {
        Setup();
    }

    //Actual game piece move happens here
    public void MovePiece(IGamePiece piece, int moveToTile)
    {
        int tileNum = piece.GetCurrentTile().SquareNumber;
        SetOccupier(null, tileNum);
        BoardSquare tile = GetBoardSquare(moveToTile);
        piece.SetCurrentTile(tile);
        SetOccupier(piece, moveToTile);
    }



    public void UnmovePiece(IGamePiece piece, int moveToTile)
    {

    }

    internal void SetOccupier(IGamePiece piece, int tile)
    {
        int index = tile - 1;
        if (piece == null)
        {
            occupier[index] = null;
            return;
        }

        if (occupier[index] != null)
        {
            //Debug.LogError("Square already occupied!");
            //throw new InvalidOperationException(
            //    $"Tile {tile} already occupied.");
            return;
        }

        occupier[index] = piece;
    }

    public IGamePiece GetOccupier(int tile)
    {
        return occupier[tile - 1];
    }

    public BoardSquare GetTileBeneath(GameObject obj)   //RaycastHit? getTileBeneath(GameObject obj)
    {
        int layerMask = LayerMask.GetMask(TILE_PLANE_LAYER);
        if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hitinfo, 10.0f, layerMask))
        {
            if (hitinfo.collider.TryGetComponent<BoardSquare>(out BoardSquare tile))
                return tile;
        }
        return null;
    }


    public BoardSquare GetBoardSquare(int num)
    {
        if (!InBounds(num))
        {
            //EXCEPTION
            Debug.Log("BoardPlane.GetBoardSquare() Exception");
            return null;
        }
        num -= 1;
        return board[num / 8, num % 8];
    }


    public static bool InBounds(int squareNum)
    {
        int index = squareNum - 1;
        if (index >= 0 && index < SIZE)
            return true;
        return false;
    }






    //~~~~~~~~~~~BOARD SETUP BELOW~~~~~~~~~~~~~
    private void ArrayToDict()
    {
        for (int i = 0; i < tileLayer.Length; i++)
        {
            if (!tileLayer[i].TryGetComponent<BoardSquare>(out BoardSquare tile))
            {
                Debug.Log("FAILED!");
            }
            boardDict.Add(tile.SquareNumber, tile);
        }
        //DEALLOCATE tileLayer!! AFTER SETUP, DEALLOCATE boardDict!!
    }


    private void Setup()
    {
        ArrayToDict();
        for (int i = 0; i < SIDE_LEN; i++)
        {
            for (int j = 0; j < SIDE_LEN; j++)
            {
                int squareNumber = SIDE_LEN * i + j + 1;
                if (!boardDict.TryGetValue(squareNumber, out BoardSquare tile))
                {
                    //didnt find
                }
                if (i == 0 && j > 0 && j < SIDE_LEN)    //BOTTOM ROW w/o CORNERS
                {
                    SetAdjTile(tile, Movement.NEXT_N);
                    SetAdjTile(tile, Movement.NEXT_W);
                    SetAdjTile(tile, Movement.NEXT_E);
                }
                else if (i == 0 && j == 0)       //BOTTOM RIGHT CORNER
                {
                    SetAdjTile(tile, Movement.NEXT_N);
                    SetAdjTile(tile, Movement.NEXT_W);
                }
                else if (i == 0 && j == SIDE_LEN - 1)    //BOTTOM LEFT CORNER
                {
                    SetAdjTile(tile, Movement.NEXT_N);
                    SetAdjTile(tile, Movement.NEXT_E);
                }
                else if (i == SIDE_LEN - 1 && j > 0 && j < SIDE_LEN)   //TOP ROW w/o CORNERS
                {
                    SetAdjTile(tile, Movement.NEXT_S);
                    SetAdjTile(tile, Movement.NEXT_W);
                    SetAdjTile(tile, Movement.NEXT_E);

                }
                else if (i == SIDE_LEN - 1 && j == 0)     //TOP RIGHT CORNER
                {
                    SetAdjTile(tile, Movement.NEXT_S);
                    SetAdjTile(tile, Movement.NEXT_W);
                }
                else if (i == SIDE_LEN - 1 && j == SIDE_LEN - 1) //TOP LEFT CORNER
                {
                    SetAdjTile(tile, Movement.NEXT_S);
                    SetAdjTile(tile, Movement.NEXT_E);
                }
                else if (i > 0 && i < SIDE_LEN && j == SIDE_LEN - 1)  //FAR LEFT COLUMN
                {
                    SetAdjTile(tile, Movement.NEXT_N);
                    SetAdjTile(tile, Movement.NEXT_S);
                    SetAdjTile(tile, Movement.NEXT_E);
                }
                else if (i > 0 && i < SIDE_LEN && j == 0) //FAR RIGHT COLUMN
                {
                    SetAdjTile(tile, Movement.NEXT_N);
                    SetAdjTile(tile, Movement.NEXT_S);
                    SetAdjTile(tile, Movement.NEXT_W);
                }
                else
                {//all
                    SetAdjTile(tile, Movement.NEXT_N);
                    SetAdjTile(tile, Movement.NEXT_S);
                    SetAdjTile(tile, Movement.NEXT_E);
                    SetAdjTile(tile, Movement.NEXT_W);
                }
                SetInitialOccupiers(tile);
                board[i, j] = tile;
            }
        }
    }


    private void SetAdjTile(BoardSquare tile, int dir)
    {
        int squareNumber = tile.SquareNumber + dir;
        if (!boardDict.TryGetValue(squareNumber, out BoardSquare adjTile))
        {
            //didnt find
        }
        if (adjTile != null)//need?
            tile.SetRight(adjTile.GetComponent<BoardSquare>());
    }

    private void SetInitialOccupiers(BoardSquare tile)
    {
        int tileNum = tile.SquareNumber;
        switch (tileNum)
        {
            case 1:
                occupier[tileNum - 1] = GameObject.Find("White Rook R").GetComponent<IGamePiece>();
                break;
            case 2:
                occupier[tileNum - 1] = GameObject.Find("White Knight R").GetComponent<IGamePiece>();
                break;
            case 3:
                occupier[tileNum - 1] = GameObject.Find("White Bishop R").GetComponent<IGamePiece>();
                break;
            case 4:
                occupier[tileNum - 1] = GameObject.Find("White King").GetComponent<IGamePiece>();
                break;
            case 5:
                occupier[tileNum - 1] = GameObject.Find("White Queen").GetComponent<IGamePiece>();
                break;
            case 6:
                occupier[tileNum - 1] = GameObject.Find("White Bishop L").GetComponent<IGamePiece>();
                break;
            case 7:
                occupier[tileNum - 1] = GameObject.Find("White Knight L").GetComponent<IGamePiece>();
                break;
            case 8:
                occupier[tileNum - 1] = GameObject.Find("White Rook L").GetComponent<IGamePiece>();
                break;
            case 9:
                occupier[tileNum - 1] = GameObject.Find("White Pawn 1").GetComponent<IGamePiece>();
                break;
            case 10:
                occupier[tileNum - 1] = GameObject.Find("White Pawn 2").GetComponent<IGamePiece>();
                break;
            case 11:
                occupier[tileNum - 1] = GameObject.Find("White Pawn 3").GetComponent<IGamePiece>();
                break;
            case 12:
                occupier[tileNum - 1] = GameObject.Find("White Pawn 4").GetComponent<IGamePiece>();
                break;
            case 13:
                occupier[tileNum - 1] = GameObject.Find("White Pawn 5").GetComponent<IGamePiece>();
                break;
            case 14:
                occupier[tileNum - 1] = GameObject.Find("White Pawn 6").GetComponent<IGamePiece>();
                break;
            case 15:
                occupier[tileNum - 1] = GameObject.Find("White Pawn 7").GetComponent<IGamePiece>();
                break;
            case 16:
                occupier[tileNum - 1] = GameObject.Find("White Pawn 8").GetComponent<IGamePiece>();
                break;
                //Black pieces start here
            case 49:
                occupier[tileNum - 1] = GameObject.Find("Black Pawn 1").GetComponent<IGamePiece>();
                break;
            case 50:
                occupier[tileNum - 1] = GameObject.Find("Black Pawn 2").GetComponent<IGamePiece>();
                break;
            case 51:
                occupier[tileNum - 1] = GameObject.Find("Black Pawn 3").GetComponent<IGamePiece>();
                break;
            case 52:
                occupier[tileNum - 1] = GameObject.Find("Black Pawn 4").GetComponent<IGamePiece>();
                break;
            case 53:
                occupier[tileNum - 1] = GameObject.Find("Black Pawn 5").GetComponent<IGamePiece>();
                break;
            case 54:
                occupier[tileNum - 1] = GameObject.Find("Black Pawn 6").GetComponent<IGamePiece>();
                break;
            case 55:
                occupier[tileNum - 1] = GameObject.Find("Black Pawn 7").GetComponent<IGamePiece>();
                break;
            case 56:
                occupier[tileNum - 1] = GameObject.Find("Black Pawn 8").GetComponent<IGamePiece>();
                break;
            case 57:
                occupier[tileNum - 1] = GameObject.Find("Black Rook R").GetComponent<IGamePiece>();
                break;
            case 58:
                occupier[tileNum - 1] = GameObject.Find("Black Knight R").GetComponent<IGamePiece>();
                break;
            case 59:
                occupier[tileNum - 1] = GameObject.Find("Black Bishop R").GetComponent<IGamePiece>();
                break;
            case 60:
                occupier[tileNum - 1] = GameObject.Find("Black King").GetComponent<IGamePiece>();
                break;
            case 61:
                occupier[tileNum - 1] = GameObject.Find("Black Queen").GetComponent<IGamePiece>();
                break;
            case 62:
                occupier[tileNum - 1] = GameObject.Find("Black Bishop L").GetComponent<IGamePiece>();
                break;
            case 63:
                occupier[tileNum - 1] = GameObject.Find("Black Knight L").GetComponent<IGamePiece>();
                break;
            case 64:
                occupier[tileNum - 1] = GameObject.Find("Black Rook L").GetComponent<IGamePiece>();
                break;
            default:
                break;
        }
    }
}