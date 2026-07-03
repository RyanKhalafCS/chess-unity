using UnityEngine;

public class PlayerManager
{
    public Player[] Players { get; private set; }
    private readonly bool testGame = false;


    public PlayerManager(Color color, bool testGame)
    {
        this.testGame = testGame;
        SetPlayers(color);
    }

    private void SetPlayers(Color color)
    {
        Players = new Player[2];
        if (color == Color.White)
        {
            Players[0] = new HumanPlayer(color, testGame);
            Players[1] = new AIPlayer(Color.Black, testGame);
        }
        else
        {
            Players[0] = new AIPlayer(Color.White, testGame);
            Players[1] = new HumanPlayer(color, testGame);
        }
    }
}