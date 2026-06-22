using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGamePiece
{
    void Initialize(GameRules rules, BoardPlane board);
    Player.Color GetColor();
    void SetPossibleMoves();
    List<BoardSquare> GetPossibleMoves();
    List<BoardSquare> GetPossibleAttacks();
    #nullable enable
    void SetCurrentTile(BoardSquare? obj);
    BoardSquare GetCurrentTile();
    bool ValidateMove(BoardSquare move);
    void DisableGamePiece();
    GameObject GetObj();
}