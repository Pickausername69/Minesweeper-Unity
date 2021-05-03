using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDecorator : IVisual
{
    private Visualizer visual;
    private bool precheck;
    private Vector2Int resolution;
    public VisualDecorator(Visualizer visual, bool precheck)
    {
        this.visual = visual;
        this.precheck = precheck;
    }
    public void HighlightTiles(Vector2Int[] tiles, AIDataType aitype)
    {
        if (precheck)
        {
            resolutioncheck();
            visual.HighlightTiles(tiles, aitype);
        }
    }

    public void SetUpVisualMinesweeper()
    {
        if (precheck)
        {
            visual.SetUpVisualMinesweeper();
            resolutioncheck();
        }
    }

    public void UpdateVisualMinesweeperComplete(MinesweeperGamestate gamestate)
    {
        if (precheck)
        {
            resolutioncheck();
            visual.UpdateVisualMinesweeperComplete(gamestate);
        }
    }

    public void UpdateVisualMinesweeperQuick(KeyValuePair<Vector2Int, MinesweeperElementInfo>[] ToUpdate)
    {
        if (precheck)
        {
            resolutioncheck();
            visual.UpdateVisualMinesweeperQuick(ToUpdate);
        }
    }
    public void FinalizeUpdate()
    {
        if (precheck)
        {
            resolutioncheck();
            visual.FinalizeUpdate();
        }
    }
    private void resolutioncheck()
    {
        
        Vector2Int newres = new Vector2Int(Screen.width, Screen.height);
        if (precheck && resolution != newres)
        {
            resolution = newres;
            Vector2 newvals;
            float[] screenxyratio =new float[] { (float)newres.x / newres.y, (float)newres.y/newres.x };
            float[] boardxyratio = new float[] { (float)visual.BoardSize.x / visual.BoardSize.y, (float)visual.BoardSize.y / visual.BoardSize.x };

            if (boardxyratio[0] / screenxyratio[0] < boardxyratio[1] / screenxyratio[1])
            {
                newvals = new Vector2(((float)visual.BoardSize.x/visual.BoardSize.y)*newres.y, newres.y);
                visual.VisualSize = new Vector2(newvals.x / (visual.TileSize.x * visual.BoardSize.x), newvals.y / (visual.TileSize.y * visual.BoardSize.y));
            }
            else 
            {
                newvals = new Vector2(newres.x, ((float)visual.BoardSize.y / visual.BoardSize.x) * newres.x);
                visual.VisualSize = new Vector2(newvals.x / (visual.TileSize.x * visual.BoardSize.x), newvals.y / (visual.TileSize.y * visual.BoardSize.y));
            }
            visual.MinesweepergameImageParent.transform.localScale = visual.VisualSize;
        }
    }

    public void HighlightMouse() 
    {
    
    }
    public Vector2Int GetMouseTilePosition(Vector2 inputpos) 
    {
        return new Vector2Int();
    }
}
