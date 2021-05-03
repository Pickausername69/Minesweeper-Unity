using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisual
{
    void HighlightTiles(Vector2Int[] tiles, AIDataType aitype);
    void UpdateVisualMinesweeperComplete(MinesweeperGamestate gamestate);
    void UpdateVisualMinesweeperQuick(KeyValuePair<Vector2Int, MinesweeperElementInfo>[] ToUpdate);
    void SetUpVisualMinesweeper();
    void FinalizeUpdate();

}
