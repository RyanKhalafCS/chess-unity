using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGamePiece
{
    void Initialize(BoardPlane board);
    Color GetColor();
    void SetPossibleMoves();
    List<BoardSquare> GetPossibleMoves();
    List<BoardSquare> GetPossibleAttacks();
    #nullable enable
    void SetCurrentTile(BoardSquare? obj);
    BoardSquare GetCurrentTile();
    bool ValidateMove(BoardSquare move);
    void DisableGamePiece();
    GameObject GetObj();
    void OnMoved();
    void TestInit(Color color);
}