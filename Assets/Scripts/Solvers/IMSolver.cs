using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMSolver
{
    void GetBoard(MinesweeperElementInfo[,] table);
    void GetRelevantBoard(KeyValuePair<Vector2Int, MinesweeperElementInfo>[] newtiles, MinesweeperActionType mat);
    KeyValuePair<MinesweeperActionType, Vector2Int> ChooseStep();
    //void AnswerProcecessor(KeyValuePair<AIRequestType, KeyValuePair<Vector2Int, MinesweeperElementInfo>[]>[] answer);

}
