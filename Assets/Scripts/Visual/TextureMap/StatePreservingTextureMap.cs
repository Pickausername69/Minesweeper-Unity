using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// TextureMap that saves only chnge
/// </summary>
/// <typeparam name="T"></typeparam>
public class StatePreservingTextureMap<T> : TextureMap<T>
{
    public T[][] representationstate;
    public StatePreservingTextureMap(GameObject Parent, GameObject InstObj, Dictionary<T, Color32[]> changeTextures, Vector2Int MapSize, Vector2Int TextureSize, Vector2Int? ChunkSize = null, FilterMode? fmode = null, bool transparentsetup = false) 
        : base(Parent, InstObj, changeTextures, MapSize, TextureSize, ChunkSize, fmode, transparentsetup)
    {
        representationstate = new T[MapSize.x][];

        for (int i = 0; i < MapSize.x; i++)
        {
            representationstate[i] = new T[MapSize.y];
        }
    }
}
