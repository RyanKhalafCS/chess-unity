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
    private GameManager gm;//use scriptable obj i think
    //private GameRules rules;
    //private BoardPlane board;
    private bool initialized = false;
    private bool processingMove = false;
    //[SerializeField] private bool componentExists;


    private void Awake()
    {
        //hasSelected = false;
        //componentExists = true;
        originalMaterial = Resources.Load(ORIGINAL_MATERIAL) as Material;
        newMaterial = Resources.Load(HIGHLIGHT) as Material;
        _startPos = transform.position;
        _currentPos = transform.position;
        _moving = false;
        highlightedTiles = new List<BoardSquare>();
    }

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }
        GameObject gmObj = GameObject.FindGameObjectWithTag("Game Manager");
        if (gmObj == null)
        {
            ExceptionHandler.PrintException(ExceptionHandler.GAMEOBJECT_NOT_FOUND_EXCEPTION);
            ExceptionHandler.ForceQuit();
            return;
        }
        if(!gmObj.TryGetComponent<GameManager>(out GameManager gm))
        {
            ExceptionHandler.PrintException(ExceptionHandler.COMPONENT_NOT_FOUND_EXCEPTION);
            ExceptionHandler.ForceQuit();
            return;
        }
        this.gm = gm;//manager;
        initialized = true;
    }

    private void OnMouseDown()
    {
        if (!enabled)// || !componentExists)
        { return; }
        if (!gameObject.TryGetComponent<IGamePiece>(out IGamePiece piece))
        {
            return;
        }

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
        //if (processingMove)
        //    return;
        //processingMove = true;

        BoardSquare moveToTile = gm.Board.GetTileBeneath(gameObject);
        GameRules rules = gm?.Game?.Rules;
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
            gm.RequestMove(gameObject, moveToTile);
        }
        else
        {//return to original pos
            ReturnPos(piece);
        }
        
        Cleanup();
    }

    private void ReturnPos(IGamePiece piece)
    {
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