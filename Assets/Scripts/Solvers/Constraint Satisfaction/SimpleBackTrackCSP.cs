using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBackTrackCSP : BaseSimpleCSP
{
    public SimpleBackTrackCSP(Vector2Int[] operators, Vector2Int BoardSize, BaseGuessor Guessor, float MineDensity, int MaxSolvingLength) : base(operators, BoardSize, Guessor, MineDensity, MaxSolvingLength)
    {
        bruteforce = new BackTrack(Closed, HiddenClosedRelations, ClosedHiddenNeighbores);
    }
}
