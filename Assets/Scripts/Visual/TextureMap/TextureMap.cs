using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// divide the originally huge texture into smaller pieces to reduce cost when texture is changed
/// </summary>
/// <typeparam name="T">the type of id for textures</typeparam>
public class TextureMap<T>
{
    public GameObject Parent;
    private GameObject[][] chunkObjs;
    private Texture2D[][] textures;

    public readonly Vector2Int MapSize;
    public readonly Vector2Int ChunkSize;
    public readonly Vector2Int TextureSize;

    public Dictionary<T, Color32[]> changeTextures;
    private List<Vector2Int> toUpdate;
    private bool[][] alreadyUpdated;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Parent">parent of the smaller textures</param>
    /// <param name="InstObj">smaller texture</param>
    /// <param name="changeTextures"></param>
    /// <param name="MapSize"></param>
    /// <param name="TextureSize"></param>
    /// <param name="ChunkSize">size of a single chunk which contains TextureSize * ChunkSize pixels (default is (1, 1))</param>
    /// <param name="transparentsetup">the default color of texture2d is white, so should the default white color be overwritten be a transparent one</param>
    public TextureMap(GameObject Parent, GameObject InstObj, Dictionary<T, Color32[]> changeTextures, Vector2Int MapSize, Vector2Int TextureSize, Vector2Int? ChunkSize = null,FilterMode? fmode=null, bool transparentsetup = false)
    {
        if (Parent is null) { throw new System.ArgumentNullException("'Parent' should not be null"); }
        else if (InstObj is null) { throw new System.ArgumentNullException("'InitObj' should not be null"); }

        if (changeTextures is null || changeTextures.Count == 0) { Debug.LogWarning("'changeTexture' is not suposed to be empty, but this class is usable regardless"); }
        if (MapSize.x <= 0 || MapSize.y <= 0) { Debug.LogWarning("'MapSize' was set to 0 or less (set back to (0, 0)"); MapSize = new Vector2Int(); }
        if (TextureSize.x <= 0 || TextureSize.y <= 0) { Debug.LogWarning("'TextureSize' was set to 0 or less (set back to (1, 1)"); TextureSize = new Vector2Int(1, 1); }

        this.Parent = Parent;
        this.MapSize = MapSize;
        Vector2Int localChunkSize = ChunkSize ?? new Vector2Int(1,1);
        this.ChunkSize = localChunkSize;
        this.TextureSize = TextureSize;
        this.changeTextures = changeTextures;

        toUpdate = new List<Vector2Int>();


        //initialize chunks
        Vector2Int chunks = new Vector2Int(MapSize.x / localChunkSize.x, MapSize.y / localChunkSize.y);
        chunks.x += chunks.x * localChunkSize.x == MapSize.x ? 0 : 1;
        chunks.y += chunks.y * localChunkSize.y == MapSize.y ? 0 : 1;

        alreadyUpdated = new bool[chunks.x][];
        for (int i = 0; i < chunks.x; i++)
        {
            alreadyUpdated[i] = new bool[chunks.y];
        }

        chunkObjs = new GameObject[chunks.x][];
        for (int i = 0; i < chunks.x; i++)
        {
            chunkObjs[i] = new GameObject[chunks.y];
        }

        textures = new Texture2D[chunks.x][];
        for (int i = 0; i < chunks.x; i++)
        {
            textures[i]= new Texture2D[chunks.y];
        }

        //transparent texture creation
        Color32[] transparent = new Color32[TextureSize.x * localChunkSize.x * TextureSize.y * localChunkSize.y];
        if (transparentsetup)
        {
            for (int i = 0; i < transparent.Length; i++)
            {
                transparent[i] = new Color(0, 0, 0, 0);
            }
        }

        //setting chunks
        for (int x = 0; x < chunks.x; x++)
        {
            for (int y = 0; y < chunks.y; y++)
            {
                GameObject currentobj = GameObject.Instantiate(InstObj, Parent.transform);
                chunkObjs[x][y] = currentobj;
                //setting recttransform
                RectTransform crect = currentobj.GetComponent<RectTransform>();
                Vector2Int currentchunksize =
                    new Vector2Int(
                        (x == chunks.x - 1 && MapSize.x - (x + 1) * localChunkSize.x != 0) ? MapSize.x - x * localChunkSize.x : localChunkSize.x,
                        (y == chunks.y - 1 && MapSize.y - (y + 1) * localChunkSize.y != 0) ? MapSize.y - y * localChunkSize.y : localChunkSize.y);
                crect.localPosition = new Vector2((currentchunksize.x * TextureSize.x) / 2, (currentchunksize.y * TextureSize.y) / 2);
                crect.anchorMin = new Vector2((localChunkSize.x / (float)MapSize.x) * x, (localChunkSize.y / (float)MapSize.y) * y);
                crect.anchorMax = crect.anchorMin;
                crect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentchunksize.x * TextureSize.x);
                crect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentchunksize.y * TextureSize.y);
                Texture2D currenttexture = new Texture2D(TextureSize.x * currentchunksize.x, TextureSize.y * currentchunksize.y);
                currenttexture.filterMode = fmode is null ? currenttexture.filterMode : fmode.Value;
                currentobj.GetComponent<Image>().sprite = Sprite.Create(currenttexture, new Rect(0, 0, TextureSize.x * currentchunksize.x, TextureSize.y * currentchunksize.y), new Vector2(0.5f, 0.5f));
                textures[x][y] = currenttexture;

                if (transparentsetup)
                {
                    if (currentchunksize == localChunkSize)
                    {
                        currenttexture.SetPixels32(transparent);
                    }
                    else 
                    {
                        Color32[] exptransparent = new Color32[TextureSize.x * currentchunksize.x * TextureSize.y * currentchunksize.y];
                        for (int i = 0; i < exptransparent.Length; i++)
                        {
                            transparent[i] = new Color(0, 0, 0, 0);
                        }
                        currenttexture.SetPixels32(exptransparent);
                    }
                    currenttexture.Apply();
                }
            }
        }
    }
    /// <summary>
    /// Update complete map, but still have to finalize changes afterwords
    /// </summary>
    /// <param name="changeids">ids refering to Keys in 'changeTextures'</param>
    public virtual void UpdateComplete(IEnumerable<T> changeids)
    {
        int index = 0;
        int cposx = 0;
        int cposy = 0;
        Vector2Int cchunk = new Vector2Int();

        //update textures
        foreach (var item in changeids)
        {
            cposx = index % MapSize.x;
            cposy = index / MapSize.x;
            cchunk.x = cposx / ChunkSize.x;
            cchunk.y = cposy / ChunkSize.y;
            textures[cchunk.x][cchunk.y].SetPixels32(
                (cposx - (cchunk.x * ChunkSize.x)) * TextureSize.x,
                (cposy - (cchunk.y * ChunkSize.y)) * TextureSize.y,
                TextureSize.x,
                TextureSize.y,
                changeTextures[item]
                );
            index++;
        }

        //add textures to 'toUpdate', because texture2d.apply need to be calles in order to change the textures
        for (int x = 0; x < textures.Length; x++)
        {
            for (int y = 0; y < textures[x].Length; y++)
            {
                if (!alreadyUpdated[x][y])
                {
                    toUpdate.Add(new Vector2Int(x, y));
                    alreadyUpdated[x][y] = true;
                }
            }

        }
    }
    #region updateat
    /// <summary>
    /// Updates the texture data at ('x', 'y')
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="changeid"></param>
    public virtual void UpdateAt(int x, int y, T changeid)
    {
        Vector2Int chunk = new Vector2Int( x / ChunkSize.x, y / ChunkSize.y);
        if (!alreadyUpdated[chunk.x][chunk.y])
        {
            toUpdate.Add(chunk);
            alreadyUpdated[chunk.x][chunk.y] = true;
        }
        textures[chunk.x][chunk.y].SetPixels32(
            (x - (chunk.x * ChunkSize.x)) * TextureSize.x,
            (y - (chunk.y * ChunkSize.y)) * TextureSize.y,
            TextureSize.x,
            TextureSize.y,
            changeTextures[changeid]
            );       
    }
    /// <summary>
    /// Updates the texture data at 'position'
    /// </summary>
    /// <param name="position"></param>
    /// <param name="changeid"></param>
    public virtual void UpdateAt(Vector2Int position, T changeid)
    {
        Vector2Int chunk = new Vector2Int(position.x / ChunkSize.x, position.y / ChunkSize.y);
        if (!alreadyUpdated[chunk.x][chunk.y])
        {
            toUpdate.Add(chunk);
            alreadyUpdated[chunk.x][chunk.y] = true;
        }
        textures[chunk.x][chunk.y].SetPixels32(
            (position.x - (chunk.x * ChunkSize.x)) * TextureSize.x,
            (position.y - (chunk.y * ChunkSize.y)) * TextureSize.y,
            TextureSize.x,
            TextureSize.y,
            changeTextures[changeid]
            );
    }
    #endregion
    #region uniqueupdateat
    /// <summary>
    /// Updates the texture data at 'position' with 'texture'
    /// </summary>
    /// <param name="position"></param>
    /// <param name="texture"></param>
    public virtual void UniqueUpdateAt(Vector2Int position, Color32[] texture)
    {
        Vector2Int chunk = new Vector2Int(position.x / ChunkSize.x, position.y / ChunkSize.y);
        if (!alreadyUpdated[chunk.x][chunk.y])
        {
            toUpdate.Add(chunk);
            alreadyUpdated[chunk.x][chunk.y] = true;
        }
        textures[chunk.x][chunk.y].SetPixels32(
            (position.x - (chunk.x * ChunkSize.x)) * TextureSize.x,
            (position.y - (chunk.y * ChunkSize.y)) * TextureSize.y,
            TextureSize.x,
            TextureSize.y,
            texture
            );
    }

    /// <summary>
    /// Updates the texture data at ('x', 'y') with 'texture'
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="texture"></param>
    public virtual void UniqueUpdateAt(int x, int y, Color32[] texture)
    {
        Vector2Int chunk = new Vector2Int(x / ChunkSize.x, y / ChunkSize.y);
        if (!alreadyUpdated[chunk.x][chunk.y])
        {
            toUpdate.Add(chunk);
            alreadyUpdated[chunk.x][chunk.y] = true;
        }
        textures[chunk.x][chunk.y].SetPixels32(
            (x - (chunk.x * ChunkSize.x)) * TextureSize.x,
            (y - (chunk.y * ChunkSize.y)) * TextureSize.y,
            TextureSize.x,
            TextureSize.y,
            texture
            );
    }
    /// <summary>
    /// Updates the texture data at 'position' with 'texture'
    /// </summary>
    /// <param name="position"></param>
    /// <param name="texture"></param>
    public virtual void UniqueUpdateAt(Vector2Int position, Color[] texture)
    {
        Vector2Int chunk = new Vector2Int(position.x / ChunkSize.x, position.y / ChunkSize.y);
        if (!alreadyUpdated[chunk.x][chunk.y])
        {
            toUpdate.Add(chunk);
            alreadyUpdated[chunk.x][chunk.y] = true;
        }
        textures[chunk.x][chunk.y].SetPixels(
            (position.x - (chunk.x * ChunkSize.x)) * TextureSize.x,
            (position.y - (chunk.y * ChunkSize.y)) * TextureSize.y,
            TextureSize.x,
            TextureSize.y,
            texture
            );
    }

    /// <summary>
    /// Updates the texture data at ('x', 'y') with 'texture'
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="texture"></param>
    public virtual void UniqueUpdateAt(int x, int y, Color[] texture)
    {
        Vector2Int chunk = new Vector2Int(x / ChunkSize.x, y / ChunkSize.y);
        if (!alreadyUpdated[chunk.x][chunk.y])
        {
            toUpdate.Add(chunk);
            alreadyUpdated[chunk.x][chunk.y] = true;
        }
        textures[chunk.x][chunk.y].SetPixels(
            (x - (chunk.x * ChunkSize.x)) * TextureSize.x,
            (y - (chunk.y * ChunkSize.y)) * TextureSize.y,
            TextureSize.x,
            TextureSize.y,
            texture
            );
    }
    #endregion
    /// <summary>
    /// Calls texture.apply for all the modified textures.
    /// Without this method no texture would be changed
    /// </summary>
    public void UpdateFinalize()
    {
        for (int i = 0; i < toUpdate.Count; i++)
        {
            Vector2Int current = toUpdate[i];
            alreadyUpdated[current.x][current.y] = false;
            textures[current.x][current.y].Apply();
        }
        toUpdate.Clear();
    }
    /// <summary>
    /// returns requested chunk
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public GameObject GetChunkOfTile(int x, int y)
    {
        return chunkObjs[x / ChunkSize.x][y / ChunkSize.y];
    }
}
