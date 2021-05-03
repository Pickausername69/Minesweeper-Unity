using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGuessor : IGuessor
{
    private bool specialfirstguess;
    private Vector2Int firstguess;
    public BaseMinesweeperSolver bms;
    protected BaseGuessor(BaseMinesweeperSolver bms)
    {
        specialfirstguess = false;
        this.bms = bms;
    }
    public BaseGuessor(BaseMinesweeperSolver bms, Vector2Int firstguess)
    {
        specialfirstguess = true;
        this.firstguess = firstguess;
        this.bms = bms;
    }
    public Vector2Int Guess() 
    {
        if (specialfirstguess)
        {
            specialfirstguess = false;
            return firstguess;
        }
        return guess();
    }
    protected abstract Vector2Int guess();
}
