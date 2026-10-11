using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BoardPlane : MonoBehaviour
{
    private bool _initialized = false;
    public const int SIZE = 64;
    public const int SIDE_LEN = 8;
    private const string TILE_PLANE_LAYER = "Tile Plane";
    private readonly BoardSquare[,] _board = new BoardSquare[8, 8];
    [FormerlySerializedAs("tileLayer")] [SerializeField] private GameObject[] _tileLayer = new GameObject[64];
    private readonly Dictionary<int, BoardSquare> _boardDict = new();
    private readonly IGamePiece[] _occupier = new IGamePiece[64];
    public bool Testing = false;

    public void Initialize(bool testing)
    {
        if (_initialized)
            return;
        this.Testing = testing;
        ArrayToDict();

        BuildBoard();

        _initialized = true;
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


    //Used for MoveSim. Use this as params for UnmovePiece. Make MovePiece return this type.
    public struct MoveSnapshot
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



    public void UnmovePiece(IGamePiece newPiece, int newPieceActualTile,
        IGamePiece actualOccupier, int move)
    {
        SetOccupier(newPiece, newPieceActualTile);
        newPiece.SetCurrentTile(GetBoardSquare(newPieceActualTile));
        SetOccupier(null, move);
        SetOccupier(actualOccupier, move);
        actualOccupier?.SetCurrentTile(GetBoardSquare(move));//null check + setCurrentTile()!! 
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
        if (_occupier[index] != null && piece != null)
        {
            throw new System.InvalidOperationException($"Square {tile} already occupied");
        }

        // Place (or clear) the occupier regardless of that piece's possible moves/attacks.
        _occupier[index] = piece;
    }

    public IGamePiece GetOccupier(int tile)
    {
        return _occupier[tile - 1];
    }

    public BoardSquare GetTileBeneath(GameObject obj)   //RaycastHit? getTileBeneath(GameObject obj)
    {
        int layerMask = LayerMask.GetMask(TILE_PLANE_LAYER);
        if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hitInfo, 10.0f, layerMask))
        {
            if (hitInfo.collider.TryGetComponent<BoardSquare>(out BoardSquare tile))
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
        BoardSquare tile = _board[num / 8, num % 8];
        return tile;
    }


    public static bool InBounds(int squareNum)
    {
        int index = squareNum - 1;
        if (index < 0 || index >= SIZE)
            return false;
        return true;
    }






    //Fills the 8x8 board array from the square dictionary.
    //Pieces claim their own starting squares in GamePiece.Initialize().
    private void BuildBoard()
    {
        for (int i = 0; i < SIDE_LEN; i++)
        {
            for (int j = 0; j < SIDE_LEN; j++)
            {
                int squareNumber = SIDE_LEN * i + j + 1;
                if (!_boardDict.TryGetValue(squareNumber, out BoardSquare tile))
                {
                    ExceptionHandler.PrintException(-1);
                    Debug.Log($"Missing BoardSquare {squareNumber} in BoardPlane.BuildBoard()!");
                    ExceptionHandler.ForceQuit();
                    continue;
                }
                _board[i, j] = tile;
            }
        }
    }


    private void ArrayToDict()
    {
        _boardDict.Clear();

        for (int i = 0; i < _tileLayer.Length; i++)
        {
            if (!_tileLayer[i].TryGetComponent<BoardSquare>(out BoardSquare tile))
            {
                Debug.Log($"FAILED to get BoardSquare at index {i}");
                continue;
            }

            int key = tile.SquareNumber;
            if (_boardDict.ContainsKey(key))
            {
                //Debug.LogWarning($"Duplicate SquareNumber {key} at index {i} - skipping");
                continue;
            }

            _boardDict.Add(key, tile);
        }
        //DEALLOCATE tileLayer!! AFTER SETUP, DEALLOCATE boardDict!!
    }
}
