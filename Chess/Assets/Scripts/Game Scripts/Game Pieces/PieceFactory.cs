//using UnityEngine;

//public class PieceFactory
//{
//    private const string BLACK_PAWN_PF = "Prefabs/White Pawn";      //.prefab";
//    private const string BLACK_ROOK_PF = "Prefabs/White Rook";      //.prefab";
//    private const string BLACK_KNIGHT_PF = "Prefabs/White Knight";      //.prefab";
//    private const string BLACK_BISHOP_PF = "Prefabs/White Bishop";      //.prefab";
//    private const string BLACK_QUEEN_PF = "Prefabs/White Queen";        //.prefab";
//    private const string BLACK_KING_PF = "Prefabs/White King";      //.prefab";

//    private const string WHITE_PAWN_PF = "Prefabs/Black Pawn";      //.prefab";
//    private const string WHITE_ROOK_PF = "Prefabs/Black Rook";      //.prefab";
//    private const string WHITE_KNIGHT_PF = "Prefabs/Black Knight";      //.prefab";
//    private const string WHITE_BISHOP_PF = "Prefabs/Black Bishop";      //.prefab";
//    private const string WHITE_QUEEN_PF = "Prefabs/Black Queen";        //.prefab";
//    private const string WHITE_KING_PF = "Prefabs/Black King";      //.prefab";


//    private static GameObject LoadPiecePrefab(Player.Color color, string whitePath, string blackPath)
//    {
//        string path = color == Player.Color.White ? whitePath : blackPath;
//        return Resources.Load<GameObject>(path);
//    }

//    public static GameObject CreatePiece(IGamePiece pieceType, Player.Color color)
//    {
//        return GetPrefab(pieceType, color);
//    }

//    private static GameObject GetPrefab(IGamePiece pieceType, Player.Color color)
//    {
//        string path = color == Player.Color.White ? whitePath : blackPath;
//        return Resources.Load<GameObject>(path);
//    }

//    private static void LoadPrefabs();

//    private static string GetPrefabPath(PieceType pieceType, Player.Color color);
//}
