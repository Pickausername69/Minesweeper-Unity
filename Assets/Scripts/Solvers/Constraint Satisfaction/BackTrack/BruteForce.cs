using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BruteForce : BaseBruteForce
{
    protected BitArray localbitarrays;
    public BruteForce(Dictionary<Vector2Int, int> closedrefs, Dictionary<Vector2Int, List<Vector2Int>> hiddenneighborerefs, Dictionary<Vector2Int, List<Vector2Int>> closedneighborerefs) : base(closedrefs, hiddenneighborerefs, closedneighborerefs)
    {
    }

    public override void Solve()
    {
        Vector2Int[] hiddens = hiddenneighborerefs.Keys.ToArray();
        Solve(hiddens);
    }

    public void Solve(Vector2Int[] hiddens) {
        Solutions.Clear();

        int combs = 0;
        localbitarrays = new BitArray(hiddens.Length, false);
        BitArray cbitarray = localbitarrays;
        while (cbitarray.TryIncrementBitArray())
        {
            combs++;
            List<Vector2Int> ccombination = new List<Vector2Int>();
            for (int i = 0; i < hiddens.Length; i++)
            {
                if (cbitarray[i])
                {
                    ccombination.Add(hiddens[i]);
                }
            }
            if (IsSatisfyingCombination(ccombination, closedrefs))
            {
                Solutions.Add(ccombination);
            }
        }
        //Debug.Log(combs + " combination");  
    }
}

