using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperSettings
{
    //game
    public readonly int x;
    public readonly int y;
    public readonly int NumberOfMines;
    //rules, and settings
    public readonly MinesweeperFirstStepRule FirstStepRule;
    public readonly Vector2Int[] Operators;
    //visual
    public readonly bool Rendering;
    //AI
    public readonly MinesweeperUsageMode usageMode;
    public readonly int SampleSize;
    public readonly int SleepTime;
    public readonly float DefaultSolvingSpeed;
    //AI settings
    public readonly MinesweeperSolverType SolverType;
    public readonly GuessorType guessorType;
    public readonly float MGDSSP_UncoverMax;
    public readonly Vector2Int FirstGuess;
    public readonly int MaxSolvingLength;
    /// <summary>
    /// Operator={(-1,-1)...(1,1)}
    /// FirstGuess=(-1,-1) understood as invalid
    /// </summary>
    public MinesweeperSettings(int lengthX = 9, int lengthY = 9, int numberOfMines = 10,
        MinesweeperFirstStepRule firstStepRule = MinesweeperFirstStepRule.SafeTile,
        bool rendering = true, MinesweeperUsageMode usageMode = MinesweeperUsageMode.NormalAI,
        int sampleSize = 0, int sleepTime = 0,
        MinesweeperSolverType solverType = MinesweeperSolverType.Standard_DSSP, GuessorType guessorType = GuessorType.Random,
        float MGDSSP_UncoverMax = 0, float DefaultSolvingSpeed = 0,
        int MaxSolvingLength = 10
        )
    {
        x = lengthX;
        y = lengthY;
        NumberOfMines = numberOfMines;
        FirstStepRule = firstStepRule;
        Rendering = rendering;
        this.usageMode = usageMode;
        SampleSize = sampleSize;
        SleepTime = sleepTime;
        this.SolverType = solverType;
        this.guessorType = guessorType;
        this.Operators = new Vector2Int[8]
        {
            new Vector2Int(0,1),
            new Vector2Int(1,1),
            new Vector2Int(1,0),
            new Vector2Int(1,-1),
            new Vector2Int(0,-1),
            new Vector2Int(-1,-1),
            new Vector2Int(-1,0),
            new Vector2Int(-1,1)
        };
        this.MGDSSP_UncoverMax = MGDSSP_UncoverMax;
        this.FirstGuess = new Vector2Int(-1, -1);
        this.DefaultSolvingSpeed = DefaultSolvingSpeed;
        this.MaxSolvingLength = MaxSolvingLength;
    }
    public MinesweeperSettings(Vector2Int[] Operators, Vector2Int FirstGuess,int lengthX = 9, int lengthY = 9, int numberOfMines = 10,
    MinesweeperFirstStepRule firstStepRule = MinesweeperFirstStepRule.SafeSouranding,
    bool rendering = true, MinesweeperUsageMode usageMode = MinesweeperUsageMode.NormalAI,
    int sampleSize = 0, int sleepTime = 0,
    MinesweeperSolverType solverType = MinesweeperSolverType.Standard_DSSP, GuessorType guessorType = GuessorType.Random,
    float MGDSSP_UncoverMax = 0, float DefaultSolvingSpeed = 0,
    int MaxSolvingLength = 10
        )
    {
        x = lengthX;
        y = lengthY;
        NumberOfMines = numberOfMines;
        FirstStepRule = firstStepRule;
        Rendering = rendering;
        this.usageMode = usageMode;
        SampleSize = sampleSize;
        SleepTime = sleepTime;
        this.SolverType = solverType;
        this.guessorType = guessorType;
        this.Operators = Operators;
        this.MGDSSP_UncoverMax = MGDSSP_UncoverMax;
        this.FirstGuess = FirstGuess;
        this.DefaultSolvingSpeed = DefaultSolvingSpeed;
        this.MaxSolvingLength = MaxSolvingLength;
    }

    public MinesweeperSettings(Vector2Int FirstGuess, int lengthX = 9, int lengthY = 9, int numberOfMines = 10,
    MinesweeperFirstStepRule firstStepRule = MinesweeperFirstStepRule.SafeSouranding,
    bool rendering = true, MinesweeperUsageMode usageMode = MinesweeperUsageMode.NormalAI,
    int sampleSize = 0, int sleepTime = 0,
    MinesweeperSolverType solverType = MinesweeperSolverType.Standard_DSSP, GuessorType guessorType = GuessorType.Random,
    float MGDSSP_UncoverMax = 0, float DefaultSolvingSpeed = 0,
    int MaxSolvingLength = 10
    )
    {
        x = lengthX;
        y = lengthY;
        NumberOfMines = numberOfMines;
        FirstStepRule = firstStepRule;
        Rendering = rendering;
        this.usageMode = usageMode;
        SampleSize = sampleSize;
        SleepTime = sleepTime;
        this.SolverType = solverType;
        this.guessorType = guessorType;
        this.Operators = new Vector2Int[8]
        {
            new Vector2Int(0,1),
            new Vector2Int(1,1),
            new Vector2Int(1,0),
            new Vector2Int(1,-1),
            new Vector2Int(0,-1),
            new Vector2Int(-1,-1),
            new Vector2Int(-1,0),
            new Vector2Int(-1,1)
        };
        this.MGDSSP_UncoverMax = MGDSSP_UncoverMax;
        this.FirstGuess = FirstGuess;
        this.DefaultSolvingSpeed = DefaultSolvingSpeed;
        this.MaxSolvingLength = MaxSolvingLength;
    }
}
public enum MinesweeperUsageMode
{
    ClickToStepAI = 0,
    NormalAI = 1
}
public enum MinesweeperSolverType
{
    Standard_DSSP,
    BorderGuessing_DSSP_WithMineGuessing,
    BorderGuessing_DSSP_WithUncovering,
    Enhanced_DSSP,
    SimpleBruteForce_CSP,
    SimpleBackTrack_CSP
}
