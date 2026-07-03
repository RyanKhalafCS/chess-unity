using UnityEngine;

public class GamePiece
{
    private static GameObject LoadPiecePrefab(Color color, string whitePath, string blackPath)
    {
        string path = color == Color.White ? whitePath : blackPath;
        return Resources.Load<GameObject>(path);
    }
}
