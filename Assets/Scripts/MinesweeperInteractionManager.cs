using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MinesweeperInteractionManager
{
    /// <summary>
    /// The first action commited by the player
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="firststep"></param>
    /// <param name="rule"></param>
    /// <param name="operators"></param>
    public static Vector2Int[] FirstOnReveal(MinesweeperGamestate tiles, Vector2Int firststep, MinesweeperFirstStepRule rule, Vector2Int[] operators)
    {
        //creating local variable resuting in miminimal performance boost
        int maxx = tiles.LengthX;
        int maxy = tiles.LengthY;

        if (rule == MinesweeperFirstStepRule.SafeSouranding)
        {
            //finding the 'safe tiles'
            List<Vector2Int> safe = new List<Vector2Int>();
            for (int i = 0; i < operators.Length; i++)
            {
                Vector2Int current = firststep + operators[i];
                if (MinesweeperElementInfo.InBounds(current,maxx,maxy))
                {
                    safe.Add(current);
                }
            }
            safe.Add(firststep);
            //remove mines from 'safe tiles'
            List<Vector2Int> bombpositions = new List<Vector2Int>();
            for (int i = 0; i < safe.Count; i++)
            {
                if (tiles[safe[i]].value >= 9)
                {
                    tiles[safe[i]].value = 0;
                    bombpositions.Add(safe[i]);
                }
            }
            //recalculate value of mines inside, and the souranding of 'safe' tiles
            for (int i = 0; i < bombpositions.Count; i++)
            {
                for (int j = 0; j < operators.Length; j++)
                {
                    Vector2Int current = bombpositions[i] + operators[j];
                    if (MinesweeperElementInfo.InBounds(current, maxx, maxy))
                    {
                        if (tiles[current].value >= 9)
                        {
                            tiles[bombpositions[i]].value++;
                        }
                        else if (!bombpositions.Contains(current))
                        {
                            tiles[current].value--;
                        }
                    }
                }
            }
            //reassigning mines [not fixed time], but faster
            List<Vector2Int> newbombs = new List<Vector2Int>();
            while (bombpositions.Count != newbombs.Count)
            {
                Vector2Int current = new Vector2Int(Random.Range(0, maxx), Random.Range(0, maxy));
                if (tiles[current].value < 9 && !safe.Contains(current))
                {
                    tiles[current].value = 9;
                    newbombs.Add(current);
                }
            }
            //recalculate souranding of the new mines
            for (int i = 0; i < newbombs.Count; i++)
            {
                for (int j = 0; j < operators.Length; j++)
                {
                    Vector2Int current = newbombs[i] + operators[j];
                    if (MinesweeperElementInfo.InBounds(current, maxx, maxy))
                    {
                        tiles[current].value++;
                    }
                }
            }
        }
        else if (rule == MinesweeperFirstStepRule.SafeTile)
        {
            //firststep
            if (tiles[firststep].value >= 9)
            {
                tiles[firststep].value = 0;
                //recalculate value of firststep, and it's souranding
                for (int i = 0; i < operators.Length; i++)
                {
                    Vector2Int current = firststep + operators[i];
                    if (MinesweeperElementInfo.InBounds(current, maxx, maxy))
                    {
                        if (tiles[current].value >= 9)
                        {
                            tiles[firststep].value++;
                        }
                        else
                        {
                            tiles[current].value--;
                        }
                    }
                }
                //reassigning mine, and recalculate it's souranding
                while (true)
                {
                    Vector2Int current = new Vector2Int(Random.Range(0, maxx), Random.Range(0, maxy));
                    if (tiles[current].value < 9 && current != firststep)
                    {
                        tiles[current].value = 9;
                        for (int i = 0; i < operators.Length; i++)
                        {
                            Vector2Int current2 = current + operators[i];
                            if (MinesweeperElementInfo.InBounds(current2, maxx, maxy))
                            {
                                tiles[current2].value++;
                            }
                        }
                        break;
                    }
                }
                if (tiles[firststep].value >= 9) { throw new System.Exception(); }
            }
        }
        return OnReveal(tiles, firststep, operators);
    }
    public static Vector2Int[] OnStep(KeyValuePair<MinesweeperActionType, Vector2Int> step, MinesweeperGamestate tiles, Vector2Int[] operators)
    {
        if (step.Key == MinesweeperActionType.Uncover)
        {
            return OnReveal(tiles, step.Value, operators);
        }
        else
        {
            return  OnFlag(tiles, step.Value);
        }
    }
    private static Vector2Int[] OnReveal(MinesweeperGamestate tiles,Vector2Int tile, Vector2Int[] operators) 
    {
        //creating local variable resuting in mminimal performance boost
        int maxx = tiles.LengthX;
        int maxy = tiles.LengthY;

        List<Vector2Int> NeedUpdate = new List<Vector2Int>();
        List<Vector2Int> opentiles=new List<Vector2Int>();
        if (tiles[tile].hidden && !tiles[tile].flaged)
        {
            if (tiles[tile].value == 0)
            {
                opentiles.Add(tile);
            }
            else if (tiles[tile].value >= 9)
            {
                Debug.Log("Game Over");
                tiles.GameOver = true;
            }
            tiles[tile].hidden = false;
            NeedUpdate.Add(tile);
        }
        while (opentiles.Count != 0)
        {
            for (int i = 0; i < operators.Length; i++)
            {
                Vector2Int current = opentiles[0] + operators[i];
                if (MinesweeperElementInfo.InBounds(current,maxx,maxy))
                { 
                    if (tiles[current].hidden)
                    {
                        tiles[current].hidden = false;
                        NeedUpdate.Add(current);
                        if (tiles[current].value == 0)
                        {
                            opentiles.Add(current);
                        }
                    }
                }
            }
            opentiles.RemoveAt(0);
        }
        return NeedUpdate.ToArray();
    }
    private static Vector2Int[] OnFlag(MinesweeperGamestate tiles, Vector2Int tile) 
    {
        tiles[tile].flaged = true;
        if (tiles[tile].value < 9) { Debug.Log("Non-mine flaged, Game Over"); tiles.GameOver = true; }
        return new Vector2Int[] { tile };
    }

}
