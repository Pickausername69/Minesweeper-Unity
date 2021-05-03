using System.Collections.Generic;
using UnityEngine;

public static class AIRequestProvider
{
    public static KeyValuePair<AIDataType, KeyValuePair<Vector2Int, MinesweeperElementInfo>[]>[]
        GetRequestedMinesweeperElementInfos(MinesweeperGamestate gamestate, KeyValuePair<AIDataType, Vector2Int[]>[] request)
    {
        KeyValuePair<AIDataType, KeyValuePair<Vector2Int, MinesweeperElementInfo>[]>[] newanswer
            = new KeyValuePair<AIDataType, KeyValuePair<Vector2Int, MinesweeperElementInfo>[]>[request.Length];
        for (int i = 0; i < request.Length; i++)
        {
            KeyValuePair<Vector2Int, MinesweeperElementInfo>[] currentrequest = MinesweeperElementInfo.GetFilteredTiles(gamestate, request[i].Value);
            newanswer[i] = new KeyValuePair<AIDataType, KeyValuePair<Vector2Int, MinesweeperElementInfo>[]>
                (
                request[i].Key,
                currentrequest
                );
        }
        return newanswer;
    }
}
