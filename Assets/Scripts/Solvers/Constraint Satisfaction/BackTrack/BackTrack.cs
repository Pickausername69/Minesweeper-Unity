using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BackTrack : BaseBruteForce
{
    private Queue<Vector2Int> NextElements;
    private HashSet<Vector2Int> AlreadyFoundCloseds;
    private Dictionary<Vector2Int, int> allcloseds = new Dictionary<Vector2Int, int>();
    private HashSet<Vector2Int> uniquehiddens = new HashSet<Vector2Int>();
    private List<Vector2Int> hiddensinorder = new List<Vector2Int>();

    public BackTrack(Dictionary<Vector2Int, int> closedrefs, Dictionary<Vector2Int, List<Vector2Int>> hiddenneighborerefs, Dictionary<Vector2Int, List<Vector2Int>> closedneighborerefs) : base(closedrefs, hiddenneighborerefs, closedneighborerefs)
    {

    }

    public override void Solve()
    {
        Solutions.Clear();
        AlreadyFoundCloseds = new HashSet<Vector2Int>();
        NextElements = new Queue<Vector2Int>();
        allcloseds = new Dictionary<Vector2Int, int>();
        uniquehiddens = new HashSet<Vector2Int>();
        hiddensinorder = new List<Vector2Int>();
        Vector2Int first = closedrefs.First().Key;
        AlreadyFoundCloseds.Add(first);
        SolveNext(first);
    }

    private void SolveNext(Vector2Int newclosed)
    {
        allcloseds.Add(newclosed, closedrefs[newclosed]);
        List<Vector2Int> validnewhiddens = AddNewElementsToHiddens(newclosed, allcloseds, uniquehiddens, hiddensinorder);
        List<BitArray> newcombinations = CreateNewCombinations(validnewhiddens);
        UpdateSolutions(newcombinations, validnewhiddens);
        if (NextElements.Count != 0)
        {
            SolveNext(NextElements.Dequeue());
        }
        //run out of current subset
        else if (closedrefs.Count != AlreadyFoundCloseds.Count) 
        {
            foreach (var item in closedrefs)
            {
                if (!AlreadyFoundCloseds.Contains(item.Key)) 
                {
                    AlreadyFoundCloseds.Add(item.Key);
                    SolveNext(item.Key);
                }
            }
        }
    }

    private List<Vector2Int> AddNewElementsToHiddens(Vector2Int newclosed, Dictionary<Vector2Int, int> allcloseds, HashSet<Vector2Int> uniquehiddens, List<Vector2Int> hiddensinorder) {
        List<Vector2Int> newhiddens = closedneighborerefs[newclosed];
        List<Vector2Int> validnewhiddens = new List<Vector2Int>();
        foreach (var item in newhiddens)
        {
            if (!uniquehiddens.Contains(item)) 
            {
                uniquehiddens.Add(item);
                hiddensinorder.Add(item);
                validnewhiddens.Add(item);
                foreach (var newcloseds in hiddenneighborerefs[item])
                {

                    if (!AlreadyFoundCloseds.Contains(newcloseds))
                    {
                        NextElements.Enqueue(newcloseds);
                        AlreadyFoundCloseds.Add(newcloseds);
                    }
                }
            }
        }
        return validnewhiddens;
    }

    private List<BitArray> CreateNewCombinations(List<Vector2Int> hiddens) {
        List<BitArray> combinations = new List<BitArray>();
        BitArray cbitarray = new BitArray(hiddens.Count);
        do
        {
            combinations.Add(new BitArray(cbitarray));
        } while (cbitarray.TryIncrementBitArray());
        return combinations;
    }

    private void UpdateSolutions(List<BitArray> newcombinations, List<Vector2Int> validnewhiddens) {
        List<List<Vector2Int>> newsolutions = new List<List<Vector2Int>>();
        if (Solutions.Count == 0)
        {
            newsolutions = CreateFreshSolutions(newcombinations, validnewhiddens);
        }
        else
        {
            foreach (var solution in Solutions)
            {
                foreach (var combination in newcombinations)
                {
                    List<Vector2Int> current = CombineToNewPossibleSolution(solution, combination, validnewhiddens);
                    if (IsSatisfyingCombination(current, allcloseds))
                    {
                        newsolutions.Add(current);
                    }
                }
            }
        }
        /*
        foreach (var item in newsolutions)
        {
            string p = "";
            item.ForEach(x => p += x);
            Debug.Log("passed: " + p);
        }*/
        Solutions = newsolutions;
    }

    private List<List<Vector2Int>> CreateFreshSolutions(List<BitArray> newcombinations, List<Vector2Int> validnewhiddens) 
    {
        List<List<Vector2Int>> newsolutions = new List<List<Vector2Int>>();
        foreach (var combination in newcombinations)
        {
            List<Vector2Int> current = CombineToNewPossibleSolution(new List<Vector2Int>(), combination, validnewhiddens);
            if (IsSatisfyingCombination(current, allcloseds))
            {
                newsolutions.Add(current);
            }
        }
        return newsolutions;
    }

    private List<Vector2Int> CombineToNewPossibleSolution(List<Vector2Int> oldsolution, BitArray newcombination, List<Vector2Int> validnewhiddens) {
        List<Vector2Int> newSolution = oldsolution.ToList();
        for (int i = 0; i < newcombination.Length; i++)
        {
            if (newcombination[i])
            {
                newSolution.Add(validnewhiddens[i]);
            }
        }
        return newSolution;
    }
}
