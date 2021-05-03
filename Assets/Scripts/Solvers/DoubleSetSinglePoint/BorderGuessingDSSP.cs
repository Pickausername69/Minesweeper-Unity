using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DSSP_ButRandomBorderGuess : AlgorithmFrame
{
    private bool guessmine;
    private OpenGuessor openGuessor;
    public DSSP_ButRandomBorderGuess(Vector2Int[] operators, Vector2Int BoardSize, BaseGuessor Guessor, bool guessmine) : base(operators, BoardSize, Guessor)
    {
        this.guessmine = guessmine;
        openGuessor = new OpenGuessor(this);
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
                Vector2Int v2i = openGuessor.Guess();
                Open.Remove(v2i);
                if (guessmine)
                {
                    return new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Flag, v2i);
                }
                else
                {
                    return new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Uncover, v2i);
                }
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
}
