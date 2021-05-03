using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class AlgorithmFrame : BaseMinesweeperSolver
{
    public AlgorithmFrame(Vector2Int[] operators, Vector2Int BoardSize, BaseGuessor Guessor) : base(operators, BoardSize, Guessor){}
    #region getboard
    /// <summary>
    /// Checks the whole board 
    /// </summary>
    /// <param name="table"> The whole minesweeper table</param>
    public override void GetBoard(MinesweeperElementInfo[,] table)
    {
        List<Vector2Int> newpositions = new List<Vector2Int>();
        for (int y = 0; y < table.GetLength(1); y++)
        {
            for (int x = 0; x < table.GetLength(0); x++)
            {
                MinesweeperElementInfo current = table[x, y];
                Vector2Int currentpos = new Vector2Int(x, y);
                if (!current.hidden && current.value > 0)
                {
                    if (!Closed.ContainsKey(currentpos))
                    {
                        newpositions.Add(currentpos);
                    }
                }
            }
        }
        for (int i = 0; i < newpositions.Count; i++)
        {
            for (int j = 0; j < Operators.Length; j++)
            {
                Vector2Int current = newpositions[i] + Operators[j];
                //in bounds
                if (current.x >= 0 && current.y >= 0 && current.x < table.GetLength(1) && current.y < table.GetLength(0))
                {
                    if (table[current.x, current.y].hidden && !table[current.x, current.y].flaged && !Open.ContainsKey(current))
                    {
                        Open.Add(current, 0);
                    }
                }
            }
            Closed.Add(newpositions[i], table[newpositions[i].x, newpositions[i].y].value);
        }
    }
    /// <summary>
    /// Updates only the tiles directly affected by an action, send List of needed elements 
    /// (Only for when revealing new tiles)
    /// </summary>
    /// <param name="toupdate">Information about newly revealed tiles</param>
    /// <param name="mat">The newly revealed tiles of the minesweeper table</param>
    public override void GetRelevantBoard(KeyValuePair<Vector2Int, MinesweeperElementInfo>[] toupdate, MinesweeperActionType mat)
    {
        if (mat == MinesweeperActionType.Uncover)
        {

            for (int i = 0; i < toupdate.Length; i++)
            {
                //note that every item in toupdate are revealed
                Vector2Int currentnewrevealed = toupdate[i].Key;
                MinesweeperElementInfo currentmsei = toupdate[i].Value;

                //there is always a possibility that the newly revealed tiles are in Open or Safe (when multiple tiles revealed)
                RemoveForSafety(currentnewrevealed);
                //add non-empties to 'Closed'
                if (currentmsei.value != 0)
                {
                    Closed.Add(currentnewrevealed, currentmsei.value);

                    //iterating  through currently revealed tile, finding new tiles while trying to eliminate currentnewrevealed
                    CheckingNewRevealed(currentnewrevealed);
                    //remove from closed if after iteration value become 0 or less
                    if (Closed[currentnewrevealed] <= 0)
                    {
                        WhenRemovingClosedNeighborCheck(currentnewrevealed);
                    }
                }
                else
                {
                    InvalidTiles[currentnewrevealed.x, currentnewrevealed.y] = true;
                }
            }
        }
        else if (mat == MinesweeperActionType.Flag)
        {
            for (int i = 0; i < toupdate.Length; i++)
            {
                Vector2Int currentmine = toupdate[i].Key;
                OnAddingFlaged(currentmine);
            }
        }
    }
    #endregion
    #region updateinnerrepresenttion
    /// <summary>
    /// Find all deterministic mines and safe tiles of a single iteration
    /// </summary>
    public void FindMinesandSafeTiles()
    {
        List<Vector2Int> newmines = new List<Vector2Int>();

        //Find 'Mine' tiles
        foreach (var item in Closed)
        {
            List<Vector2Int> localmine = new List<Vector2Int>();
            for (int i = 0; i < Operators.Length; i++)
            {
                Vector2Int newpos = item.Key + Operators[i];
                if (Open.ContainsKey(newpos))
                {
                    localmine.Add(newpos);
                }
            }
            if (item.Value == localmine.Count)
            {
                newmines.AddRange(localmine); 
            }
        }

        List<Vector2Int> newmines2 = new List<Vector2Int>(); 

        for (int i = 0; i < newmines.Count; i++)
        {
            Vector2Int current = newmines[i];

            if (Open.Remove(current))
            {               
                Mine.Add(current);
                newmines2.Add(current);
            }
        }

        //Find 'Safe' tiles
        for (int i = 0; i < newmines2.Count; i++)
        {
            AddingMineCheck(newmines2[i]);
        }
    }
    #endregion
    #region requestupdate
    /// <summary>
    /// This method was used before Invalidtiles
    /// </summary>
    /// <param name="answer">Data received from AIRequestProvider.GetRequestedMinesweeperElementInfos method</param>
    public void AnswerProcecessor(KeyValuePair<AIDataType, KeyValuePair<Vector2Int, MinesweeperElementInfo>[]>[] answer) 
    {
        for (int i = 0; i < answer.Length; i++)
        {
            AIDataType requestType = answer[i].Key;
            KeyValuePair<Vector2Int, MinesweeperElementInfo>[] currentanswer = answer[i].Value;

            for (int j = 0; j < currentanswer.Length; j++)
            {
                Vector2Int currenttile = currentanswer[j].Key;
                MinesweeperElementInfo currentmsei = currentanswer[j].Value;
                if (currentmsei.hidden)
                {
                    switch (requestType)
                    {
                        case AIDataType.Open:
                            {
                                //for safety, i only allow tiles to be considered safe if they were previusly open
                                //so here i check if they belong to 'Safe'
                                if (NextToInvalidTile(currenttile))
                                {
                                   Safe.Add(currenttile);
                                }
                                else
                                {
                                    Open.Add(currenttile, 0);
                                }
                                break;
                            }
                        case AIDataType.Safe:
                            {
                                Safe.Add(currenttile);
                                break;
                            }
                    }
                }
            }
        }
    }
    protected bool NextToInvalidTile(Vector2Int tile)
    {
        int maxx = BoardSize.x;
        int maxy= BoardSize.y;
        for (int i = 0; i < Operators.Length; i++)
        {
            Vector2Int current = tile + Operators[i];
            if (MinesweeperElementInfo.InBounds(current, maxx, maxy) && InvalidTiles[current.x, current.y])
            { return true; }
        }
        return false;
    }
    #endregion
    protected KeyValuePair<Vector2Int, MinesweeperElementInfo>[] GetBorderTiles(KeyValuePair<Vector2Int, MinesweeperElementInfo>[] newtiles) 
    {
        List<int> indexes = new List<int>();
        Dictionary<Vector2Int, MinesweeperElementInfo> newtiles2 = newtiles.ToDictionary(x => x.Key, x => x.Value);
        bool notborder = false;

        for (int i = 0; i < newtiles.Length; i++)
        {
            notborder = true;
            for (int j = 0; j < Operators.Length; j++)
            {
                Vector2Int current = newtiles[i].Key + Operators[j];
                if (!newtiles2.ContainsKey(current))
                {
                    notborder = false;
                    indexes.Add(i);
                    break;
                }
            }
            if (notborder)
            {
                InvalidTiles[newtiles[i].Key.x, newtiles[i].Key.y] = true;
            }
        }

        KeyValuePair<Vector2Int, MinesweeperElementInfo>[] border = new KeyValuePair<Vector2Int, MinesweeperElementInfo>[indexes.Count];
        for (int i = 0; i < border.Length; i++)
        {
            KeyValuePair<Vector2Int, MinesweeperElementInfo> current = newtiles[i];
            border[i] = newtiles[i];
        }
        return border;
    }
    /// <summary>
    /// A sigle step choosen by this algorithm
    /// </summary>
    public override abstract KeyValuePair<MinesweeperActionType, Vector2Int> ChooseStep();
    /// <summary>
    /// Iterates through 'currentnewrevealed', checking if 'currentnewrevealed' can be discarded,
    /// while adding new open tile
    /// </summary>
    /// <param name="currentnewrevealed"></param>
    protected virtual void CheckingNewRevealed(Vector2Int currentnewrevealed) 
    {
        for (int j = 0; j < Operators.Length; j++)
        {
            Vector2Int current = currentnewrevealed + Operators[j];
            if (Flaged.Contains(current))
            {
                Closed[currentnewrevealed]--;
            }
            else if (MinesweeperElementInfo.InBounds(current, BoardSize.x, BoardSize.y))
            {
                //i wanted to make the minesweeper and the solver independent from eachother, thats why i didn't simply passed the minesweeperelementinfo object at 'current' tile
                if (!InvalidTiles[current.x, current.y] && !Open.ContainsKey(current) && !Closed.ContainsKey(current) && !Safe.Contains(current) && !Mine.Contains(current))
                {
                    Open.Add(current, 0);
                }
            }
        }
    }
    protected virtual void WhenRemovingClosedNeighborCheck(Vector2Int currentnewrevealed) 
    {
        for (int k = 0; k < Operators.Length; k++)
        {
            Vector2Int current = currentnewrevealed + Operators[k];

            if (Open.Remove(current))
            {
                Safe.Add(current);
            }
        }
        WhenRemovingClosed(currentnewrevealed);
    }
    protected virtual void WhenRemovingClosed(Vector2Int v2i)
    {
        Closed.Remove(v2i);
        InvalidTiles[v2i.x, v2i.y] = true;
    }
    /// <summary>
    /// tries to remove current newly reveale tile just in case
    /// </summary>
    protected virtual void RemoveForSafety(Vector2Int currentnewrevealed)
    {
        if (!Open.Remove(currentnewrevealed))
        { Safe.Remove(currentnewrevealed); }
    }

    protected virtual void OnAddingFlaged(Vector2Int currentmine)
    {
        Flaged.Add(currentmine);
    }

    protected virtual void AddingMineCheck(Vector2Int newmine) 
    {
        for (int j = 0; j < Operators.Length; j++)
        {
            Vector2Int current = newmine + Operators[j];

            int closedIntValue;

            if (Closed.TryGetValue(current, out closedIntValue))
            {
                if (closedIntValue > 0)
                {
                    Closed[current]--;
                    if ((closedIntValue - 1) <= 0)
                    {
                        WhenRemovingClosedNeighborCheck(current);
                    }
                }
                else
                {
                    throw new System.Exception("Invalid 'Closed' element found: " + current.ToString() + " " + Closed[current]);
                }
            }
        }
    }
}
public static class AlgorithmFrameExtension
{
    //used for Visualhandler.HighlightTiles method
    //only should be used if in the result previously existing tiles are never removed
    private static bool[,] prevGetPositionsResult;
    public static Vector2Int[] GetPositions(this bool[,] invalids, bool OnlyNewOnes = false)
    {
        if (OnlyNewOnes && prevGetPositionsResult is null || 
            prevGetPositionsResult.GetLength(0) != invalids.GetLength(0) ||  prevGetPositionsResult.GetLength(1) != invalids.GetLength(1)) 
        {
            prevGetPositionsResult = new bool[invalids.GetLength(0), invalids.GetLength(1)];
        }

        List<Vector2Int> inv = new List<Vector2Int>();
        for (int y = 0; y < invalids.GetLength(1); y++)
        {
            for (int x = 0; x < invalids.GetLength(0); x++)
            {
                bool current = invalids[x, y];
                bool pcurrent = prevGetPositionsResult[x, y];
                if (OnlyNewOnes)
                {
                    if (current && !pcurrent)
                    {
                        inv.Add(new Vector2Int(x, y));
                        prevGetPositionsResult[x, y] = true;
                    }
                    else if (OnlyNewOnes && !current && pcurrent)
                    {
                        resetprevGetPositionsResult();
                        return GetPositions(invalids, true);
                    }
                }
                else
                {
                    if (current)
                    {
                        inv.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return inv.ToArray();
    }

    private static void resetprevGetPositionsResult() 
    {
        for (int y = 0; y < prevGetPositionsResult.GetLength(1); y++)
        {
            for (int x = 0; x < prevGetPositionsResult.GetLength(0); x++)
            {
                prevGetPositionsResult[x, y] = false;
            }
        }
    }
}