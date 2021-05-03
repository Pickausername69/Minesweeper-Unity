using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomGuessor : BaseGuessor
{
    public RandomGuessor(BaseMinesweeperSolver bms) : base(bms){    }
    public RandomGuessor(BaseMinesweeperSolver bms, Vector2Int firstguess) : base(bms, firstguess){    }
    protected override Vector2Int guess()
    {
        List<Vector2Int> possibilities = new List<Vector2Int>();
        bool[,] invalids = bms.InvalidTiles;
        
        for (int y = 0; y < invalids.GetLength(1); y++)
        {
            for (int x = 0; x < invalids.GetLength(0); x++)
            {
                if (!invalids[x, y]) { possibilities.Add(new Vector2Int(x,y)); }
            }
        }
        return possibilities[Random.Range(0, possibilities.Count)];
    }
}
