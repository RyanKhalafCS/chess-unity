using System.Collections.Generic;
using UnityEngine;

public abstract class Player
{
    public readonly Color PlayerColor;
    public King PlayersKing { get; private set; }
    public List<IGamePiece> UnitsCheckingEnemyKing { get;  set; }
    public List<GameObject> Units => units;
    private List<GameObject> units;
    public bool Check => check;
    private bool check;
    public bool Testing { get; private set; }

    internal Player(Color color, bool test)
    {
        this.Testing = test;
        units = new List<GameObject>();
        UnitsCheckingEnemyKing = new();
        PlayerColor = color;
        check = false;
        if (Testing)
            CreateTestKing();
        else
            InitializeUnits(PlayerColor);
    }

    internal void SetInCheck(bool value)
    {
        check = value;
    }


    internal void RemoveUnit(GameObject unit)
    {
        if (unit == null)
            return;

        if(unit.TryGetComponent<IGamePiece>(out IGamePiece gPiece))
        {
            //old. remove/reconsider
            if(gPiece is Pawn 
                || gPiece is Rook
                || gPiece is Bishop
                || gPiece is Knight
                || gPiece is Queen
                || gPiece is King)
            {
                if(units.Contains(unit))
                    units.Remove(unit);
            }
        }
    }


    //HOW TO ADD SAFETY TO GameObject.Find()? Use try catch?
    private void InitializeUnits(Color color)
    {
        bool success;
        if (color == Color.White)
        {
            const string temp = "WhitePiece";
            success = InitHelper(temp);
        }
        else
        {
            const string temp = "BlackPiece";
            success = InitHelper(temp);
        }
        if(success == false)
        {
            Debug.Log("Initialization failed.\nExiting...");
            ExceptionHandler.ForceQuit();
        }
    }


    private bool InitHelper(string color)
    {
        GameObject[] colorArr = GameObject.FindGameObjectsWithTag(color);
        const int PIECE_COUNT = 16;
        if (colorArr.Length != PIECE_COUNT)
        {
            Debug.LogError($"Missing {color} pieces detected at unit init().");
            return false;
        }
        for (int i = 0; i < 16; i++)
        {
            IGamePiece king = colorArr[i].GetComponent<IGamePiece>();
            if (king is King)
            {
                PlayersKing = (King)king;
            }
            units.Add(colorArr[i]);
        }
        return true;
    }

    private void CreateTestKing()
    {
        string kingName;
        if ((PlayerColor == Color.White))
        {
            kingName = "White King";
        }
        else
        {
            kingName = "Black King";
        }
        GameObject kingObj = new(kingName);
        King king = kingObj.AddComponent<King>();
        PlayersKing = king;
        units.Add(kingObj);
    }

}