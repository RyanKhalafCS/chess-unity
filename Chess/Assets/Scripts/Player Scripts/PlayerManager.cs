using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerManager
{
    public Player[] Players { get; private set; }


    public PlayerManager(Player.Color color)
    {
        SetPlayers(color);
    }

    private void SetPlayers(Player.Color color)
    {
        Players = new Player[2];
        if (color == Player.Color.White)
        {
            Players[0] = new HumanPlayer(color);
            Players[1] = new AIPlayer(Player.Color.Black);
        }
        else
        {
            Players[0] = new AIPlayer(Player.Color.White);
            Players[1] = new HumanPlayer(color);
        }
    }
}