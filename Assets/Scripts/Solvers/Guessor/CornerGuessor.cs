using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Guess the corners first then the edges, if no edge left it will choose randomly
/// </summary>
public class CornerGuessor : BaseGuessor
{
    private List<Vector2Int> corner;
    private List<Vector2Int> edge;

    public CornerGuessor(BaseMinesweeperSolver bms, int xlength, int ylength) : base(bms)
    {
        corner = new List<Vector2Int>();
        edge = new List<Vector2Int>();

        int lastx = xlength - 1;
        int lasty = ylength - 1;

        //corner
        corner.Add(new Vector2Int(0, 0));
        corner.Add(new Vector2Int(0, lasty));
        corner.Add(new Vector2Int(lastx, 0));
        corner.Add(new Vector2Int(lastx, lasty));

        //edge
        for (int i = 1; i < lastx; i++)
        {
            edge.Add(new Vector2Int(i, 0));
            edge.Add(new Vector2Int(i, lasty));
        }
        for (int i = 0; i < lasty; i++)
        {
            edge.Add(new Vector2Int(0, i));
            edge.Add(new Vector2Int(lastx, i));
        }
    }

    public CornerGuessor(BaseMinesweeperSolver bms, int xlength, int ylength, Vector2Int firstguess) : base(bms, firstguess)
    {
        corner = new List<Vector2Int>();
        edge = new List<Vector2Int>();

        int lastx = xlength - 1;
        int lasty = ylength - 1;

        //corner
        corner.Add(new Vector2Int(0, 0));
        corner.Add(new Vector2Int(0, lasty));
        corner.Add(new Vector2Int(lastx, 0));
        corner.Add(new Vector2Int(lastx, lasty));

        //edge
        for (int i = 1; i < lastx; i++)
        {
            edge.Add(new Vector2Int(i, 0));
            edge.Add(new Vector2Int(i, lasty));
        }
        for (int i = 0; i < lasty; i++)
        {
            edge.Add(new Vector2Int(0, i));
            edge.Add(new Vector2Int(lastx, i));
        }
    }

    private Vector2Int cornerguess()
    {
        int index;
        Vector2Int choosen;

        if (corner.Count > 0)
        {
            index = Random.Range(0, corner.Count);
            choosen = corner[index];
            corner.RemoveAt(index);
        }
        else 
        {
            index = Random.Range(0, edge.Count);
            choosen = edge[index];
            edge.RemoveAt(index);
        }
        return choosen;
    }
    protected override Vector2Int guess()
    {
        //recheckcorners();
        if (edge.Count > 0 || corner.Count > 0)
        {
            return cornerguess();
        }
        List<Vector2Int> possibilities = new List<Vector2Int>();
        bool[,] invt = bms.InvalidTiles;
        for (int y = 0; y < invt.GetLength(1); y++)
        {
            for (int x = 0; x < invt.GetLength(0); x++)
            {
                if (!invt[x, y])
                {
                    possibilities.Add(new Vector2Int(x, y));
                }
            }
        }
        return possibilities[Random.Range(0, possibilities.Count)];
    }
    //too expensive to use
    private void recheckcorners() 
    {
        int index = 0;
        while (index != corner.Count)
        {
            Vector2Int current = corner[index];
            if (bms.InvalidTiles[current.x, current.y] || bms.Closed.ContainsKey(current) || bms.Flaged.Contains(current))
            {
                corner.RemoveAt(index);
            }
            else { index++; }
        }
        index = 0;
        while (index != edge.Count)
        {
            Vector2Int current = edge[index];
            if (bms.InvalidTiles[current.x, current.y] || bms.Closed.ContainsKey(current) || bms.Flaged.Contains(current))
            {
                edge.RemoveAt(index);
            }
            else { index++; }
        }
    }
}
