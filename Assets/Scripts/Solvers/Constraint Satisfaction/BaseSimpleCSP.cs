using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseSimpleCSP : AlgorithmFrame
{
    protected Dictionary<Vector2Int, List<Vector2Int>> ClosedHiddenNeighbores;
    protected Dictionary<Vector2Int, List<Vector2Int>> HiddenClosedRelations;
    protected BaseBruteForce bruteforce;
    private readonly float uncoverMaxValue;
    private readonly int MaxSolvingLength;
    public BaseSimpleCSP(Vector2Int[] operators, Vector2Int BoardSize, BaseGuessor Guessor, float MineDensity, int MaxSolvingLength) : base(operators, BoardSize, Guessor)
    {
        ClosedHiddenNeighbores = new Dictionary<Vector2Int, List<Vector2Int>>(BoardSize.x);
        HiddenClosedRelations = new Dictionary<Vector2Int, List<Vector2Int>>(BoardSize.x);
        uncoverMaxValue = MineDensity;
        this.MaxSolvingLength = MaxSolvingLength;
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
                if (Open.Count <= MaxSolvingLength)
                {
                    recalculateOpenTiles();
                }
                else 
                {
                    return new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Uncover, Guessor.Guess());
                }
            }

            if (OutOfTrivials)
            {
                KeyValuePair<Vector2Int, float> first = Open.First();
                KeyValuePair<Vector2Int, float> smallest = first;              
                foreach (var item in Open)
                {
                    if (item.Value < smallest.Value)
                    {
                        smallest = item;
                    }
                }
                if (smallest.Value < uncoverMaxValue)
                {
                    Debug.Log("backtrack choose "+smallest.ToString());
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
    protected override void CheckingNewRevealed(Vector2Int currentnewrevealed)
    {
        //count the number of neighbores of this tile
        List<Vector2Int> newClosedHiddenNeighbors = new List<Vector2Int>();
        ClosedHiddenNeighbores.Add(currentnewrevealed, newClosedHiddenNeighbors);

        for (int j = 0; j < Operators.Length; j++)
        {
            Vector2Int current = currentnewrevealed + Operators[j];
            if (Flaged.Contains(current))
            {
                Closed[currentnewrevealed]--;
            }
            else if (MinesweeperElementInfo.InBounds(current, BoardSize.x, BoardSize.y))
            {
                if (Open.ContainsKey(current) || Safe.Contains(current) || Mine.Contains(current))
                {
                    newClosedHiddenNeighbors.Add(current);
                    List<Vector2Int> clist = HiddenClosedRelations[current];
                    clist.Add(currentnewrevealed);
                }
                else if (!InvalidTiles[current.x, current.y] && !Closed.ContainsKey(current))
                {
                    Open.Add(current, 0);
                    newClosedHiddenNeighbors.Add(current);
                    List<Vector2Int> clist = new List<Vector2Int>();
                    HiddenClosedRelations.Add(current, clist);
                    clist.Add(currentnewrevealed);
                }
            }
        }
    }
    protected override void WhenRemovingClosed(Vector2Int v2i)
    {
        Closed.Remove(v2i);

        //remove all hidden from closed
        for (int i = 0; i < ClosedHiddenNeighbores[v2i].Count; i++)
        {
            Vector2Int current = ClosedHiddenNeighbores[v2i][i];
            HiddenClosedRelations[current].Remove(v2i);
            //Debug.Log("removed from: " + current);
        }
        ClosedHiddenNeighbores.Remove(v2i);
        InvalidTiles[v2i.x, v2i.y] = true;
    }

    protected override void RemoveForSafety(Vector2Int currentnewrevealed)
    {
        base.RemoveForSafety(currentnewrevealed);
        for (int i = 0; i < Operators.Length; i++)
        {
            Vector2Int current = currentnewrevealed + Operators[i];
            List<Vector2Int> clist;
            if (ClosedHiddenNeighbores.TryGetValue(current, out clist))
            {
                clist.Remove(currentnewrevealed);
            }
            HiddenClosedRelations.Remove(currentnewrevealed);
        }
    }

    protected override void OnAddingFlaged(Vector2Int currentmine)
    {
        List<Vector2Int> clist;
        if (HiddenClosedRelations.TryGetValue(currentmine, out clist))
        {
            for (int i = 0; i < clist.Count; i++)
            {
                //Debug.Log(clist[i]);
                ClosedHiddenNeighbores[clist[i]].Remove(currentmine);
            }
        }
        HiddenClosedRelations.Remove(currentmine);
        base.OnAddingFlaged(currentmine);
    }

    protected virtual void recalculateOpenTiles()
    {
        //reset values
        //(simple foreach wouldn't do, because readonly)
        var keys = Open.Keys.ToArray();
        foreach (var item in keys)
        {
            Open[item] = 0;
        }
        //recalculate values
        bruteforce.Solve();
        var results = bruteforce.GetResult();
        //assign values
        foreach (var item in results)
        {
            //Debug.Log(item.ToString() + " assigned");
            if (item.Value == 0)
            {
                Open.Remove(item.Key);
                Safe.Add(item.Key);
            }
            else if (item.Value == 1)
            {
                Open.Remove(item.Key);
                Mine.Add(item.Key);
                AddingMineCheck(item.Key);
            }
            else
            {
                Open[item.Key] = item.Value;
            }
        }
    }
    /// <summary>
    /// Should only be used if there is no element in Mine and Safe
    /// </summary>
    protected void RelationalSimplyfication()
    {
        foreach (var item in ClosedHiddenNeighbores)
        {
            
            Vector2Int testedclosed = item.Key;
            List<Vector2Int> current = item.Value;
            for (int i = 0; i < current.Count; i++)
            {
                Vector2Int searchedhidden = current[i];
                List<Vector2Int> clist = HiddenClosedRelations[searchedhidden]; //closeds next to hidden
                for (int j = 0; j < clist.Count; j++)
                {
                    Vector2Int cv2i = clist[j];
                    Debug.Log(searchedhidden.ToString()+" "+cv2i.ToString());
                    //is part of 'current'
                    if (cv2i != testedclosed && ClosedHiddenNeighbores[cv2i].All(x => current.Contains(x)))
                    {
                        Debug.Log(cv2i + " is a subset of " + testedclosed);
                        for (int k = 0; k < ClosedHiddenNeighbores[cv2i].Count; k++)
                        {
                            current.Remove(ClosedHiddenNeighbores[cv2i][k]);
                        }
                        //subtract mines
                        Closed[testedclosed] -= Closed[cv2i];

                        if (Closed[testedclosed] == 0)
                        {
                            current.ForEach(y =>
                            {
                                Open.Remove(y);
                                Safe.Add(y);
                            });
                            //[error] caused by whenremovingclosed
                            for (int k = 0; k < ClosedHiddenNeighbores[testedclosed].Count; k++)
                            {
                                Vector2Int current2 = ClosedHiddenNeighbores[testedclosed][k];
                                HiddenClosedRelations[current2].Remove(testedclosed);
                            }
                            return;
                        }
                        else if (current.Count == Closed[testedclosed])
                        {
                            Open.Remove(item.Key);
                            Mine.Add(item.Key);
                            current.ForEach(y => 
                            {
                                Open.Remove(y);
                                Mine.Add(y);
                                AddingMineCheck(y);
                            });
                            return;
                        }
                        //return;
                    }
                }
            }
        }
    }
    protected override void AddingMineCheck(Vector2Int newmine)
    {
        for (int j = 0; j < Operators.Length; j++)
        {
            Vector2Int current = newmine + Operators[j];

            if (Closed.ContainsKey(current))
            {
                if (Closed[current] > 0)
                {
                    Closed[current]--;
                    if (Closed[current] <= 0)
                    {
                        WhenRemovingClosedNeighborCheck(current);
                    }
                }
            }
        }
    }
}
