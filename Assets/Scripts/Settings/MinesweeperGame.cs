using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MinesweeperGame
{
    private static MinesweeperGamestate _gamestate;
    public static MinesweeperGamestate Gamestate 
    {
        get 
        {
            if (_gamestate is null)
            {
                _gamestate = new MinesweeperGamestate(Settings.x, Settings.y, Settings.Operators, Settings.NumberOfMines);
            }
            return _gamestate;
        }
        set
        {
            _gamestate = value;
        }
    }
    private static MinesweeperSettings _settings;
    public static MinesweeperSettings Settings 
    {
        get 
        {
            if (_settings is null)
            {
                MinesweeperSettings ms = new MinesweeperSettings();
                _settings = ms;
            }
            return _settings; 
        }
        set 
        {
           _settings = value;
        } 
    }
    public static void CreateNewGame()
    {
        Gamestate.GenerateNewGame();
    }





}
public enum MinesweeperFirstStepRule
{
    CanBeMine = 0,
    SafeTile = 1,
    SafeSouranding = 2
}
public enum MinesweeperActionType
{
    Uncover = 0,
    Flag = 1
}