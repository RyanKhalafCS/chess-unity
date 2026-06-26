using UnityEngine;
using System;
using System.Collections.Generic;

public class SelectPiece : MonoBehaviour
{
    private Material newMaterial;
    private Material originalMaterial;
    private const string ORIGINAL_MATERIAL = "Materials/Clear";
    private const string HIGHLIGHT = "Materials/Highlight";
    private const string ATTACK_HIGHLIGHT = "Materials/AttackHighlight";
    private Vector3 _startPos;
    private Vector3 _currentPos;
    private bool _moving;
    private List<BoardSquare> highlightedTiles;
    //private GameRules rules;
    //private BoardPlane board;
    //private bool initialized = false;


    private void Awake()
    {
        //hasSelected = false;
        originalMaterial = Resources.Load(ORIGINAL_MATERIAL) as Material;
        newMaterial = Resources.Load(HIGHLIGHT) as Material;
        _startPos = transform.position;
        _currentPos = transform.position;
        _moving = false;
        highlightedTiles = new List<BoardSquare>();
    }

    //public void Initialize(BoardPlane board)
    //{
    //    if (initialized)
    //    {
    //        return;
    //    }
    //    this.board = board;
    //    initialized = true;
    //}

    private void OnMouseDown()//record original pos... rather, retrieve Pawn.getCurrentTile();
    {
        if(!gameObject.TryGetComponent<IGamePiece>(out IGamePiece piece))
        {
            return;
        }

        //for(int i = 0; i < piece.GetPossibleMoves().Count; i++) {
        //    Debug.Log("move: " + piece.GetPossibleMoves()[i]);
        //}
        piece.SetPossibleMoves();//too reduntant here? maybe move into an "if" statement?
        if (highlightedTiles.Count > 0)
        {
            UnhighlightTiles();
        }
        newMaterial = Resources.Load(HIGHLIGHT) as Material;
        HighlightTiles(piece.GetPossibleMoves());
        newMaterial = Resources.Load(ATTACK_HIGHLIGHT) as Material;
        HighlightTiles(piece.GetPossibleAttacks());
    }


    private void OnMouseDrag()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        float distanceToCamera = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, distanceToCamera));
        mouseWorldPos.y = (float)2;
        if(_moving)
        {
            transform.position = mouseWorldPos;
        }
        else
        {
            _currentPos = mouseWorldPos;
            if(Math.Abs(_startPos.x - _currentPos.x) > 1 || Math.Abs(_startPos.z - _currentPos.z) > 1)
            {
                _moving = true;
            }
        }
    }

    private void OnMouseUp()
    {
        BoardSquare moveToTile = GameManager.Instance.Board.GetTileBeneath(gameObject);
        GameRules rules = GameManager.Instance?.Game?.Rules;
        if (rules == null)//null check needed? Game game = GameManager?.Instance?.Game?;
            return;
        
        if (!gameObject.TryGetComponent<IGamePiece>(out IGamePiece piece) || moveToTile == null)
        {
            ReturnPos(piece);
        }
        else if (rules.ValidMove(piece, moveToTile) && piece.ValidateMove(moveToTile)) //create field GameManager and initialize field in Init() function. Call from GameManager.
        {
            /* if(tile.squareNumber == Movement.CanKingRookSwap(GetComponent<IGamePiece>()).squareNumber)
            {
                Movement.KingRookSwap(GetComponent<IGamePiece>());
            }*/
            GameManager.Instance.RequestMove(gameObject, moveToTile);
        }
        else
        {//return to original pos
            ReturnPos(piece);
        }
        
        UnhighlightTiles();
        _moving = false;
        _startPos = transform.position;
        _currentPos = transform.position;
    }

    private void ReturnPos(IGamePiece piece)
    {
        transform.position = piece.GetCurrentTile().transform.position;
    }


    //move this to new script later maybe
    private void HighlightTiles(List<BoardSquare> tiles)
    {
        for(int i = 0; i < tiles.Count; i++)
        {
            Renderer renderer = tiles[i].GetComponent<Renderer>();
            if(renderer != null && newMaterial != null)
                renderer.material = newMaterial;
            else
            {
                Debug.Log("Highlight failed.");
            }
        }
        foreach (BoardSquare item in tiles)
        {
            highlightedTiles.Add(item);
        }
    }

    private void UnhighlightTiles()
    {
        for(int i = 0; i < highlightedTiles.Count; i++)
        {
            Renderer renderer = highlightedTiles[i].GetComponent<Renderer>();
            if(renderer != null && originalMaterial != null)
                renderer.material = originalMaterial;
            else
            {
                Debug.Log("Highlight failed.");
            }
        }
        highlightedTiles.Clear();
    }
}