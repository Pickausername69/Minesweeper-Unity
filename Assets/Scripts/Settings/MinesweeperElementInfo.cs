using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweeperElementInfo
{
    public bool hidden = true;
    public int value = 0;
    public bool flaged = false;

    /// <summary>
    /// Checks if x, and y coordinates are inside array
    /// (maxx, and maxy are out of bounds)
    /// </summary>
    /// <param name="current"></param>
    /// <param name="maxx"></param>
    /// <param name="maxy"></param>
    /// <returns></returns>
    public static bool InBounds(Vector2Int current, int maxx, int maxy)
    {
        if (current.x >= 0 && current.y >= 0 && current.x < maxx && current.y < maxy)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Returns tiles relative to 'thistile'
    /// (Does not include 'thistile')
    /// </summary>
    /// <param name="thistile"></param>
    /// <param name="relativetiles"></param>
    /// <returns></returns>
    public static Vector2Int[] GetRelativeTiles(Vector2Int thistile, Vector2Int[] relativetiles)
    {
        Vector2Int[] abstiles = new Vector2Int[relativetiles.Length];

        for (int i = 0; i < relativetiles.Length; i++)
        {
            abstiles[i] = thistile + relativetiles[i];
        }
        return abstiles;
    }
    /// <summary>
    /// Returns part of the board requested with the 'filter' parameter
    /// </summary>
    /// <param name="board"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static KeyValuePair<Vector2Int, MinesweeperElementInfo>[] GetFilteredTiles(MinesweeperGamestate board, Vector2Int[] filter) 
    {
        KeyValuePair<Vector2Int, MinesweeperElementInfo>[] filtered = new KeyValuePair<Vector2Int, MinesweeperElementInfo>[filter.Length];

        for (int i = 0; i < filter.Length; i++)
        {
            Vector2Int current = filter[i];

            filtered[i] = new  KeyValuePair<Vector2Int, MinesweeperElementInfo>(current ,board[current]);
        }
        return filtered;
    }
}
