using UnityEngine;

public class Movement
{
    public enum MoveDir //maybe remove
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }
    public const int NEXT_N = 8;
    public const int NEXT_NW = 9;
    public const int NEXT_NE = 7;
    public const int NEXT_W = 1;
    public const int NEXT_E = -1;
    public const int NEXT_S = -8;
    public const int NEXT_SW = -7;
    public const int NEXT_SE = -9;
    public const int NUM_OF_DIRS = 8;

    


    public static bool IsOverEdge(int squareNum, int dir)
    {
        int index = squareNum - 1;
        int row = index / 8;
        int col = index % 8;

        if(dir == NEXT_N && (row == 7 ))
            return true;
        if(dir == NEXT_S && (row == 0))
            return true;
        if(dir == NEXT_E && (col == 0))
            return true;
        if(dir == NEXT_W && (col == 7 ))
            return true;
        if(dir == NEXT_NE && (col == 0 || row == 7))
            return true;
        if(dir == NEXT_NW && (col == 7 || row == 7))
            return true;
        if(dir == NEXT_SW && (col == 7 || row == 0))
            return true;
        if(dir == NEXT_SE && (col == 0 || row == 0))
            return true;

        return false;
    }


    public static int GetColorMultiplier(Color color)
    {
        if(color == Color.White)
            return 1;
        else
            return -1;
    }
}
