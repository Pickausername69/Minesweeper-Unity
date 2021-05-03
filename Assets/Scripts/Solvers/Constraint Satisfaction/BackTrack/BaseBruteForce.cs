using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class BaseBruteForce
{
    public List<List<Vector2Int>> Solutions { protected set; get; }
    protected Dictionary<Vector2Int, int> closedrefs;
    protected Dictionary<Vector2Int, List<Vector2Int>> closedneighborerefs;
    protected Dictionary<Vector2Int, List<Vector2Int>> hiddenneighborerefs;
    public BaseBruteForce(Dictionary<Vector2Int, int> closedrefs, Dictionary<Vector2Int, List<Vector2Int>> hiddenneighborerefs, Dictionary<Vector2Int, List<Vector2Int>> closedneighborerefs)
    {
        this.closedrefs = closedrefs;
        this.closedneighborerefs = closedneighborerefs;
        this.hiddenneighborerefs = hiddenneighborerefs;
        Solutions = new List<List<Vector2Int>>();
    }

    protected bool IsSatisfyingCombination(List<Vector2Int> combination, Dictionary<Vector2Int, int> allclosed)
    {
        Dictionary<Vector2Int, int> localclosed = allclosed.ToDictionary(entry => entry.Key, entry => entry.Value);
        foreach (Vector2Int current in combination)
        {
            List<Vector2Int> copenneighbores;
			//hiddenneighborerefs try to change to my own
            if (hiddenneighborerefs.TryGetValue(current, out copenneighbores))
            {
                for (int j = 0; j < copenneighbores.Count; j++)
                {
                    Vector2Int cclosed = copenneighbores[j];
                    if (localclosed.ContainsKey(cclosed))
                    {
                        localclosed[cclosed]--;
                        if (localclosed[cclosed] <= 0)
                        {
                            localclosed.Remove(cclosed);
                        }
                    }
                    else if(allclosed.ContainsKey(cclosed)) { return false; }
                }
            }
            else
            {
                return false;
            }
        }
        if (localclosed.Count == 0)
        {
            return true;
        }
        return false;
    }
    public virtual KeyValuePair<Vector2Int, float>[] GetResult()
    {
        //Debug.Log("Solutions: "+Solutions.Count);
        Dictionary<Vector2Int, int> resultcounter = new Dictionary<Vector2Int, int>();
        //count number of Vector2Int in Solutions
        for (int i = 0; i < Solutions.Count; i++)
        {
            foreach (var item in Solutions[i])
            {
                if (resultcounter.ContainsKey(item))
                {
                    resultcounter[item]++;
                }
                else
                {
                    resultcounter.Add(item, 1);
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in Solutions[i])
            {
                sb.Append(item);
            }
            //Debug.Log("Solution: "+sb.ToString());
        }

        int max = Solutions.Count;
        int index = 0;
        KeyValuePair<Vector2Int, float>[] result = new KeyValuePair<Vector2Int, float>[hiddenneighborerefs.Count];
        //get number of Vector2Int in Solutions / number of Solutions
        foreach (var item in hiddenneighborerefs)
        {
            Vector2Int current = item.Key;
            int val;
            if (resultcounter.TryGetValue(current, out val))
            {
                result[index] = new KeyValuePair<Vector2Int, float>(current, (float)val / max);
            }
            else
            {
                result[index] = new KeyValuePair<Vector2Int, float>(current, 0);
            }
            index++;
        }
        return result;
    }

    public abstract void Solve();
}
public static class BitArrayExtension
{
    public static bool TryIncrementBitArray(this BitArray bitarray)
    {
        int length = bitarray.Length;
        int index = 0;
        while (index < length)
        {
            if (bitarray[index])
            {
                bitarray[index] = false;
                index++;
            }
            else
            {
                bitarray[index] = true;
                return true;
            }
        }
        return false;
    }
    public static List<Vector2Int> GetTrueListElements(this BitArray bitArray, List<Vector2Int> list) {
        if (bitArray.Length != list.Count)
        {
            throw new System.Exception("BitArray.Length and List.Count should be equal");
        }
        List<Vector2Int> combination = new List<Vector2Int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (bitArray[i])
            {
                combination.Add(list[i]);
            }
        }
        return combination;
    }
}
