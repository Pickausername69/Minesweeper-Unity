using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBruteForceCSP : BaseSimpleCSP
{
    public SimpleBruteForceCSP(Vector2Int[] operators, Vector2Int BoardSize, BaseGuessor Guessor, float MineDensity, int MaxSolvingLength) : base(operators, BoardSize, Guessor, MineDensity, MaxSolvingLength)
    {
        bruteforce = new BruteForce(Closed, HiddenClosedRelations, ClosedHiddenNeighbores);
    }
}
