using UnityEngine;

public class PlayerManager
{
    public Player[] Players { get; private set; }
    private readonly bool _testGame = false;


    public PlayerManager(Color color, bool testGame)
    {
        _testGame = testGame;
        SetPlayers(color);
    }

    private void SetPlayers(Color color)
    {
        Players = new Player[2];
        if (color == Color.White)
        {
            Players[0] = new HumanPlayer(color, _testGame);
            Players[1] = new AIPlayer(Color.Black, _testGame);
        }
        else
        {
            Players[0] = new AIPlayer(Color.White, _testGame);
            Players[1] = new HumanPlayer(color, _testGame);
        }
    }
}