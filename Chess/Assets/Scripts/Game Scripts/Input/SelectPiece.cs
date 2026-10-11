using UnityEngine;
using System;
using System.Collections.Generic;

public class SelectPiece : MonoBehaviour
{
    private Material _newMaterial;
    private Material _originalMaterial;
    private const string ORIGINAL_MATERIAL = "Materials/Clear";
    private const string HIGHLIGHT = "Materials/Highlight";
    private const string ATTACK_HIGHLIGHT = "Materials/AttackHighlight";
    private Vector3 _startPos;
    private Vector3 _currentPos;
    private bool _moving;
    private List<BoardSquare> _highlightedTiles;
    private GameManager _gameManager;
    private bool _initialized = false;
    private bool _processingMove = false;


    private void Awake()
    {
        _originalMaterial = Resources.Load(ORIGINAL_MATERIAL) as Material;
        _newMaterial = Resources.Load(HIGHLIGHT) as Material;
        _startPos = transform.position;
        _currentPos = transform.position;
        _moving = false;
        _highlightedTiles = new List<BoardSquare>();
    }

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }
        GameObject gameManagerObj = GameObject.FindGameObjectWithTag("Game Manager");
        if (gameManagerObj == null)
        {
            ExceptionHandler.PrintException(ExceptionHandler.GAMEOBJECT_NOT_FOUND_EXCEPTION);
            ExceptionHandler.ForceQuit();
            return;
        }
        if(!gameManagerObj.TryGetComponent<GameManager>(out GameManager gameManager))
        {
            ExceptionHandler.PrintException(ExceptionHandler.COMPONENT_NOT_FOUND_EXCEPTION);
            ExceptionHandler.ForceQuit();
            return;
        }
        _gameManager = gameManager;
        _initialized = true;
    }

    private void OnMouseDown()
    {
        if (!enabled)// || !componentExists)
        { return; }
        if (!gameObject.TryGetComponent<IGamePiece>(out IGamePiece piece))
        {
            return;
        }

        piece.SetPossibleMoves();
        if (_highlightedTiles.Count > 0)
        {
            UnhighlightTiles();
        }
        _newMaterial = Resources.Load(HIGHLIGHT) as Material;
        HighlightTiles(piece.GetPossibleMoves());
        _newMaterial = Resources.Load(ATTACK_HIGHLIGHT) as Material;
        HighlightTiles(piece.GetPossibleAttacks());
    }


    private void OnMouseDrag()
    {
        if(!enabled)// || !componentExists)
        { return; }
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
        if (!enabled)// || !componentExists)
        { return; }

        BoardSquare moveToTile = _gameManager.Board.GetTileBeneath(gameObject);
        GameRules rules = _gameManager?.Game?.Rules;
        if (rules == null)//fix later
            return;
        
        if (!gameObject.TryGetComponent<IGamePiece>(out IGamePiece piece) || moveToTile == null)
        {
            ReturnPos(piece);
        }
        else if (rules.ValidMove(piece, moveToTile) && piece.ValidateMove(moveToTile))
        {
            /* if(tile.squareNumber == Movement.CanKingRookSwap(GetComponent<IGamePiece>()).squareNumber)
            {
                Movement.KingRookSwap(GetComponent<IGamePiece>());
            }*/
            _gameManager.RequestMove(gameObject, moveToTile);
        }
        else
        {//return to original pos
            ReturnPos(piece);
        }
        
        Cleanup();
    }

    private void ReturnPos(IGamePiece piece)
    {
        if (piece == null)
            return;
        transform.position = piece.GetCurrentTile().transform.position;
    }

    public void Cleanup()
    {
        UnhighlightTiles();
        _moving = false;
        _startPos = transform.position;
        _currentPos = transform.position;
        //componentExists = false;
    }


    //move this to new script later maybe
    private void HighlightTiles(List<BoardSquare> tiles)
    {
        for(int i = 0; i < tiles.Count; i++)
        {
            Renderer renderer = tiles[i].GetComponent<Renderer>();
            if(renderer != null && _newMaterial != null)
                renderer.material = _newMaterial;
            else
            {
                Debug.Log("Highlight failed.");
            }
        }
        foreach (BoardSquare item in tiles)
        {
            _highlightedTiles.Add(item);
        }
    }

    private void UnhighlightTiles()
    {
        for(int i = 0; i < _highlightedTiles.Count; i++)
        {
            Renderer renderer = _highlightedTiles[i].GetComponent<Renderer>();
            if(renderer != null && _originalMaterial != null)
                renderer.material = _originalMaterial;
            else
            {
                Debug.Log("Highlight failed.");
            }
        }
        _highlightedTiles.Clear();
    }
}