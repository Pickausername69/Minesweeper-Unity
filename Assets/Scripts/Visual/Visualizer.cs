using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Visualizer : IVisual
{
    public Vector2 VisualSize;
    public Vector2Int TileSize;
    public readonly Vector2Int BoardSize;

    public GameObject MinesweepergameImageParent;
    public Visualizer(Vector2Int BoardSize,  GameObject MinesweepergameImageParent)
    {
        this.MinesweepergameImageParent = MinesweepergameImageParent;
        this.BoardSize = BoardSize;
    }
    public abstract void HighlightTiles(Vector2Int[] tiles, AIDataType aitype);
    public abstract void SetUpVisualMinesweeper();
    public abstract void UpdateVisualMinesweeperComplete(MinesweeperGamestate gamestate);
    public abstract void UpdateVisualMinesweeperQuick(KeyValuePair<Vector2Int, MinesweeperElementInfo>[] ToUpdate);
    public abstract void FinalizeUpdate();
}
