using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DSSP : AlgorithmFrame
{
    public DSSP(Vector2Int[] operators, Vector2Int BoardSize, BaseGuessor Guessor) : base(operators, BoardSize, Guessor){ }

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
                Vector2Int v2i = Guessor.Guess();
                return new KeyValuePair<MinesweeperActionType, Vector2Int>(MinesweeperActionType.Uncover, v2i);
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
