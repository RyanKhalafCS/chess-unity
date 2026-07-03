using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using System.Diagnostics;

public class BoardPlane : MonoBehaviour
{
    //bool initialized = false;//track if Setup() was already called once?
    public const int SIZE = 64;
    public const int SIDE_LEN = 8;
    private const string TILE_PLANE_LAYER = "Tile Plane";
    private readonly BoardSquare[,] board = new BoardSquare[8, 8];
    [SerializeField] private GameObject[] tileLayer = new GameObject[64];
    private readonly Dictionary<int, BoardSquare> boardDict = new();//<squareNum, Square>
    private readonly IGamePiece[] occupier = new IGamePiece[64];
    public bool Testing = false;

    public void Initialize(bool testing)
    {
        this.Testing = testing;
        ArrayToDict();
        if (testing)
            SetupTestBoard();
        else
            Setup();

    }


    public bool CanMove(int squareNum)
    {
        return GetOccupier(squareNum) == null;
    }


    public bool CanAttack(Color currentPiece, int squareNum)
    {
        IGamePiece atkedPiece = GetOccupier(squareNum);
        return atkedPiece != null && !currentPiece.Equals(atkedPiece.GetColor());
    }



    public struct MoveSnapshot//use this as params for UnmovePiece. Make MovePiece return this type. Allows for undoing the last move. Use list<MoveSnapshot> to enable multiple undos.
    {
        public IGamePiece MovedPiece;
        public BoardSquare From;
        public BoardSquare To;
        public IGamePiece CapturedPiece;
    }

    //Actual game piece move happens here
    public void MovePiece(IGamePiece piece, int moveToTile)
    {
        // Ensure destination is cleared (capture simulated) and record any captured piece.
        IGamePiece captured = PrepareMove(moveToTile);

        // Clear source tile (if any) before moving
        if (piece.GetCurrentTile() != null)
            SetOccupier(null, piece.GetCurrentTile().SquareNumber);

        // Move piece into destination
        piece.SetCurrentTile(GetBoardSquare(moveToTile));
        SetOccupier(piece, moveToTile);
    }



    public void UnmovePiece(IGamePiece newPiece, int newPieceActualTile, //ai complaining about dependence on newPieceActualTile not being null. Should always have value, add null checking anyway.
        IGamePiece ActualOccupier, int move)
    {
        SetOccupier(newPiece, newPieceActualTile);
        newPiece.SetCurrentTile(GetBoardSquare(newPieceActualTile));
        SetOccupier(null, move);
        SetOccupier(ActualOccupier, move);
        ActualOccupier?.SetCurrentTile(GetBoardSquare(move));//null check + setCurrentTile()!!
    }

    // Prepares moveSquare for an incoming piece
    // by removing the piece currently on that BoardSquare
    public IGamePiece PrepareMove(int moveSquare)
    {
        IGamePiece actualOccupier = null;
        if (GetOccupier(moveSquare) is not null)
        {
            actualOccupier = GetOccupier(moveSquare);
            SetOccupier(null, moveSquare);
            //actualOccupier.SetCurrentTile(null);
        }
        return actualOccupier;
    }

    public void SetOccupier(IGamePiece piece, int tile)
    {
        // validate tile index
        if (!InBounds(tile))
            throw new System.ArgumentOutOfRangeException(nameof(tile));

        int index = tile - 1;

        // If the destination already has an occupier and we're trying to place another piece, that's an error.
        if (occupier[index] != null && piece != null)
        {
            throw new System.InvalidOperationException($"Square {tile} already occupied");
        }

        // Place (or clear) the occupier regardless of that piece's possible moves/attacks.
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
            UnityEngine.Debug.Log("BoardPlane.GetBoardSquare() Exception");
            return null;
        }
        num -= 1;
        BoardSquare tile = board[num / 8, num % 8];
        return tile;
    }


    public static bool InBounds(int squareNum)
    {
        int index = squareNum - 1;
        if (index < 0 || index >= SIZE)
            return false;
        return true;
    }






    //~~~~~~~~~~~BOARD SETUP BELOW~~~~~~~~~~~~~
    //private int SetupPieces()
    //{
    //    GameObject prefab = Resources.Load<GameObject>(BLACK_PAWN_PF);
    //    for (int i = 0; i < 8; i++)
    //    {
    //        Quaternion q = Quaternion.identity;
    //        Instantiate(prefab, board[0, i].transform.position, 1);
    //    }


    //    return 0;
    //}    

    //finally going to have to fix the way game is setup. This is temp fix till then.
    public void SetupTestBoard()
    {
        for (int i = 0; i < SIDE_LEN; i++)
        {
            for (int j = 0; j < SIDE_LEN; j++)
            {
                int squareNumber = SIDE_LEN * i + j + 1;
                if (!boardDict.TryGetValue(squareNumber, out BoardSquare tile))
                {
                    //didnt find
                }

                board[i, j] = tile;
            }
        }
        //for (int i = 0; i < SIDE_LEN; i++)
        //{
        //    for (int j = 0; j < SIDE_LEN; j++)
        //    {
        //        int squareNumber = SIDE_LEN * i + j + 1;
        //        if (!InBounds(squareNumber))
        //        {
        //            break;
        //        }
        //        GameObject obj = new();
        //        board[i, j] = obj.AddComponent<BoardSquare>();
        //        obj.name = "";


        //        //
        //        //SetInitialOccupiers(tile);
        //        //board[i, j] = tile;
        //    }
        //}
    }


    private void ArrayToDict()
    {
        for (int i = 0; i < tileLayer.Length; i++)
        {
            if (!tileLayer[i].TryGetComponent<BoardSquare>(out BoardSquare tile))
            {
                UnityEngine.Debug.Log("FAILED!");
            }
            boardDict.Add(tile.SquareNumber, tile);
        }
        //DEALLOCATE tileLayer!! AFTER SETUP, DEALLOCATE boardDict!!
    }


    private void Setup()
    {
        for (int i = 0; i < SIDE_LEN; i++)
        {
            for (int j = 0; j < SIDE_LEN; j++)
            {
                int squareNumber = SIDE_LEN * i + j + 1;
                if (!boardDict.TryGetValue(squareNumber, out BoardSquare tile))
                {
                    //didnt find
                }
                SetInitialOccupiers(tile);
                board[i, j] = tile;
            }
        }
    }


    //if (i == 0 && j > 0 && j < SIDE_LEN)    //BOTTOM ROW w/o CORNERS
    //{
    //    SetAdjTile(tile, Movement.NEXT_N);
    //    SetAdjTile(tile, Movement.NEXT_W);
    //    SetAdjTile(tile, Movement.NEXT_E);
    //}
    //else if (i == 0 && j == 0)       //BOTTOM RIGHT CORNER
    //{
    //    SetAdjTile(tile, Movement.NEXT_N);
    //    SetAdjTile(tile, Movement.NEXT_W);
    //}
    //else if (i == 0 && j == SIDE_LEN - 1)    //BOTTOM LEFT CORNER
    //{
    //    SetAdjTile(tile, Movement.NEXT_N);
    //    SetAdjTile(tile, Movement.NEXT_E);
    //}
    //else if (i == SIDE_LEN - 1 && j > 0 && j < SIDE_LEN)   //TOP ROW w/o CORNERS
    //{
    //    SetAdjTile(tile, Movement.NEXT_S);
    //    SetAdjTile(tile, Movement.NEXT_W);
    //    SetAdjTile(tile, Movement.NEXT_E);

    //}
    //else if (i == SIDE_LEN - 1 && j == 0)     //TOP RIGHT CORNER
    //{
    //    SetAdjTile(tile, Movement.NEXT_S);
    //    SetAdjTile(tile, Movement.NEXT_W);
    //}
    //else if (i == SIDE_LEN - 1 && j == SIDE_LEN - 1) //TOP LEFT CORNER
    //{
    //    SetAdjTile(tile, Movement.NEXT_S);
    //    SetAdjTile(tile, Movement.NEXT_E);
    //}
    //else if (i > 0 && i < SIDE_LEN && j == SIDE_LEN - 1)  //FAR LEFT COLUMN
    //{
    //    SetAdjTile(tile, Movement.NEXT_N);
    //    SetAdjTile(tile, Movement.NEXT_S);
    //    SetAdjTile(tile, Movement.NEXT_E);
    //}
    //else if (i > 0 && i < SIDE_LEN && j == 0) //FAR RIGHT COLUMN
    //{
    //    SetAdjTile(tile, Movement.NEXT_N);
    //    SetAdjTile(tile, Movement.NEXT_S);
    //    SetAdjTile(tile, Movement.NEXT_W);
    //}
    //else
    //{//all
    //    SetAdjTile(tile, Movement.NEXT_N);
    //    SetAdjTile(tile, Movement.NEXT_S);
    //    SetAdjTile(tile, Movement.NEXT_E);
    //    SetAdjTile(tile, Movement.NEXT_W);
    //}

    //private void SetAdjTile(BoardSquare tile, int dir)
    //{
    //    int squareNumber = tile.SquareNumber + dir;
    //    if (!boardDict.TryGetValue(squareNumber, out BoardSquare adjTile))
    //    {
    //        //didnt find
    //    }
    //    if (adjTile != null)//need?
    //        tile.SetRight(adjTile.GetComponent<BoardSquare>());
    //}


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