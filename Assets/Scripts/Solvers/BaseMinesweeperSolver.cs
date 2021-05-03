using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseMinesweeperSolver : IMSolver
{
    public BaseMinesweeperSolver(Vector2Int[] operators, Vector2Int BoardSize, BaseGuessor Guessor)
    {
        Open = new Dictionary<Vector2Int, float>(BoardSize.x);
        Closed = new Dictionary<Vector2Int, int>(BoardSize.x);
        Flaged = new HashSet<Vector2Int>();
        Safe = new HashSet<Vector2Int>();
        Mine = new HashSet<Vector2Int>();
        this.Operators = operators;
        this.BoardSize = BoardSize;
        //tiles that are no longer valueable
        InvalidTiles = new bool[BoardSize.x, BoardSize.y];
        this.Guessor = Guessor;
        this.Guessor.bms = this;
    }
    /// <summary>
    /// hidden tiles on the border
    /// </summary>
    public virtual Dictionary<Vector2Int, float> Open { get; protected set; }
    /// <summary>
    /// revealed tiles on the border
    /// </summary>
    public virtual Dictionary<Vector2Int, int> Closed { get; protected set; }
    /// <summary>
    /// position of flags
    /// </summary>
    public virtual HashSet<Vector2Int> Flaged { get; protected set; }
    /// <summary>
    /// tiles that guarantied to reveal an empty tile
    /// </summary>
    public virtual HashSet<Vector2Int> Safe { get; protected set; }
    /// <summary>
    /// tiles that guarantied to be mines
    /// </summary>
    public virtual HashSet<Vector2Int> Mine { get; protected set; }
    private protected readonly Vector2Int[] Operators;
    private protected readonly Vector2Int BoardSize;
    public bool[,] InvalidTiles { get; private set; }
    private protected BaseGuessor Guessor { get; }
    public abstract void GetBoard(MinesweeperElementInfo[,] table);
    public abstract void GetRelevantBoard(KeyValuePair<Vector2Int, MinesweeperElementInfo>[] newtiles, MinesweeperActionType mat);
    public abstract KeyValuePair<MinesweeperActionType, Vector2Int> ChooseStep();
    public bool OutOfTrivials
    {
        get
        {
            return (Safe.Count + Mine.Count == 0);
        }
    }

}
public enum AIDataType
{
    None,
    Open,
    Safe,
    Closed,
    Mine,
    Invalid
}
public static class HashSetHelper 
{
    public static void AddRange<T>(this HashSet<T> hset, T[] newelements)
    {
        for (int i = 0; i < newelements.Length; i++)
        {
            hset.Add(newelements[i]);
        }
    }
    public static void AddRangeList<T>(this HashSet<T> hset, List<T> newelements)
    {
        for (int i = 0; i < newelements.Count; i++)
        {
            hset.Add(newelements[i]);
        }
    }
    public static void RemoveRangeList<T, T2>(this Dictionary<T, T2> dict, List<T> toremove)
    {
        for (int i = 0; i < toremove.Count; i++)
        {
            dict.Remove(toremove[i]);
        }
    }
}
