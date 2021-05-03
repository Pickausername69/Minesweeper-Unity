using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpenGuessor : BaseGuessor
{
    public OpenGuessor(BaseMinesweeperSolver bms) : base(bms) { }
    public OpenGuessor(BaseMinesweeperSolver bms, Vector2Int firstguess) : base(bms, firstguess) { }
    protected override Vector2Int guess()
    {
        Vector2Int[] opens2 = Enumerable.ToArray(bms.Open.Keys as IEnumerable<Vector2Int>);
        return opens2[Random.Range(0, opens2.Length)];
    }
}
