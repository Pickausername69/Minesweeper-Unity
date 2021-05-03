using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperGamestate
{
    private MinesweeperElementInfo[,] _gamestate;
    public readonly int LengthX;
    public readonly int LengthY;
    public readonly Vector2Int[] Operators;
    public readonly int NumberOfMines;
    public bool FirstStep { set; get; }
    public bool GameOver { set; get; }
    public MinesweeperGamestate(int x, int y,Vector2Int[] operators, int NumberOfMines)
    {
        _gamestate = new MinesweeperElementInfo[x, y];
        LengthX = x;
        LengthY = y;
        GameOver = false;
        this.Operators = operators;
        FirstStep = true;
        this.NumberOfMines = NumberOfMines;
    }
    public MinesweeperElementInfo this[Vector2Int index]
    {
        get => _gamestate[index.x, index.y];
        private set => _gamestate[index.x, index.y] = value;
    }
    public MinesweeperElementInfo this[int x, int y]
    {
        get => _gamestate[x, y];
        private set => _gamestate[x, y] = value;
    }
    public void GenerateNewGame()
    {
        FirstStep = true; 
        GameOver = false;
        Vector2Int[] mines = MineGenerationFixed();
        for (int y2 = 0; y2 < LengthY; y2++)
        {
            for (int x2 = 0; x2 < LengthX; x2++)
            {
                _gamestate[x2, y2] = new MinesweeperElementInfo();
            }
        }
        for (int i = 0; i < mines.Length; i++)
        {
            _gamestate[mines[i].x, mines[i].y].value = 9;
            for (int j = 0; j < Operators.Length; j++)
            {
                Vector2Int current = new Vector2Int(mines[i].x + Operators[j].x, mines[i].y + Operators[j].y);
                if (MinesweeperElementInfo.InBounds(current, LengthX, LengthY))
                {
                    _gamestate[current.x, current.y].value += 1;
                }
            }
        }
    }
    private Vector2Int[] MineGenerationFixed()
    {
        List<Vector2Int> tiles = new List<Vector2Int>(NumberOfMines);
        for (int y = 0; y < LengthY; y++)
        {
            for (int x = 0; x < LengthX; x++)
            {
                tiles.Add(new Vector2Int(x, y));
            }
        }
        Vector2Int[] mines = new Vector2Int[NumberOfMines];
        for (int i = 0; i < NumberOfMines; i++)
        {
            int current = Random.Range(0, tiles.Count);
            mines[i] = tiles[current];
            tiles.RemoveAt(current);
        }
        return mines;
    }

}
