using System;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class BoardSquare : MonoBehaviour
{
    public int SquareNumber { get; private set; }
    private BoardSquare forward;
    private BoardSquare backward;
    private BoardSquare left;
    private BoardSquare right;


    void Awake()
    {
        string[] nameParts = gameObject.name.Split(" ");//you either have to assign the id value at object creation or do this, or assign from editor. Dont want to create all objects from scratch.
        int num = int.Parse(nameParts[1]);
        SquareNumber = num;
    }

    
    public BoardSquare GetForward()
    {
        return forward;
    }
    public BoardSquare GetBackward()
    {
        return backward;
    }
    public BoardSquare GetLeft()
    {
        return left;
    }
    public BoardSquare GetRight()
    {
        return right;
    }

    public void SetForward(BoardSquare tile)
    {
        forward = tile;
    }
    
    public void SetBackward(BoardSquare tile)
    {
        backward = tile;
    }
    
    public void SetLeft(BoardSquare tile)
    {
        left = tile;
    }
    
    public void SetRight(BoardSquare tile)
    {
        right = tile;
    }
}
