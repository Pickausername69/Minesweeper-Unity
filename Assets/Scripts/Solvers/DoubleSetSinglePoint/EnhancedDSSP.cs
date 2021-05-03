using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MineGuessingDSSP : BaseSimpleCSP
{

    public float uncoverMaxValue { get; protected set; }
    public MineGuessingDSSP(Vector2Int[] operators, Vector2Int BoardSize, BaseGuessor Guessor, float uncoverMaxValue) : base(operators, BoardSize, Guessor,0,0)
    {
        ClosedHiddenNeighbores = new Dictionary<Vector2Int, List<Vector2Int>>(BoardSize.x);
        HiddenClosedRelations = new Dictionary<Vector2Int, List<Vector2Int>>(BoardSize.x);
        this.uncoverMaxValue = uncoverMaxValue;
    }
    public override KeyValuePair<MinesweeperActionType, Vector2Int> ChooseStep()
    {
        Vector2Int current;
        if (Open.Count == 0 && Safe.Count == 0 && Mine.Count == 0)
        {
            return new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Uncover, Guessor.Guess());
        }
        else
        {
            if (OutOfTrivials)
            {
                FindMinesandSafeTiles();
            }

            if (OutOfTrivials)
            {
                recalculateOpenTiles();

                KeyValuePair<Vector2Int, float> first = Open.First();
                KeyValuePair<Vector2Int, float> smallest = first;
                foreach (var item in Open)
                {
                    if (item.Value < smallest.Value)
                    {
                        smallest = item;
                    }
                }
                if (smallest.Value <= uncoverMaxValue)
                {
                    //Debug.Log("Smallest was choosen.");
                    Open.Remove(smallest.Key);
                    return new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Uncover, smallest.Key);
                }
                return new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Uncover, Guessor.Guess());
            }
            else
            {
                if (Mine.Count != 0)
                {
                    current = Mine.First();
                    Mine.Remove(current);
                    return new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Flag, current);
                }
                else
                {
                    current = Safe.First();
                    Safe.Remove(current);
                    return new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Uncover, current);
                }
            }
        }
    }

    protected override void recalculateOpenTiles()
    {
        //reset values
        //(simple foreach wouldn't do, because readonly)
        var keys = Open.Keys.ToArray();
        //Open.Clear();
        foreach (var item in keys)
        {
            Open[item] = 0;
        }
        //recalculate values
        foreach (var item in ClosedHiddenNeighbores)
        {
            Vector2Int cClosed = item.Key;
            int cCount = item.Value.Count;
            for (int i = 0; i < cCount; i++)
            {
                Open[item.Value[i]] += (float)Closed[cClosed] / cCount;
            }
        }
    }
}
