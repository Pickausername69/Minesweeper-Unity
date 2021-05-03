using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Visualhandler : Visualizer
{
    public GameObject overlayimagebutton;
    public GameObject highlightprefab;
    public Sprite minesweepersprite;

    public Dictionary<int, Color32[]> dict;
    public TextureMap<int> textureMap;

    public Texture2D[] textures { get; }
    
    public Dictionary<AIDataType, Vector2Int[]> highlighttoupdate;
    public Dictionary<AIDataType, Color32[]> highlightcolors;
    public TextureMap<AIDataType> highlighttmap;
    public Visualhandler(GameObject MinesweepergameImageParent, GameObject highlightprefab, GameObject overlayimagebutton, Vector2Int BoardSize) : base(BoardSize, MinesweepergameImageParent)
    {
        this.highlightprefab = highlightprefab;
        this.overlayimagebutton = overlayimagebutton;

        string statelibrary = "Tilestates/";

        Texture2D a = Resources.Load<Sprite>(statelibrary + "number_0.9").texture;
        TileSize = new Vector2Int(a.width, a.height);

        //load tile sprites
        textures = new Texture2D[13];
        textures[0] = a;
        textures[1] = Resources.Load<Sprite>(statelibrary + "number_1").texture;
        textures[2] = Resources.Load<Sprite>(statelibrary + "number_2").texture;
        textures[3] = Resources.Load<Sprite>(statelibrary + "number_3").texture;
        textures[4] = Resources.Load<Sprite>(statelibrary + "number_4").texture;
        textures[5] = Resources.Load<Sprite>(statelibrary + "number_5").texture;
        textures[6] = Resources.Load<Sprite>(statelibrary + "number_6").texture;
        textures[7] = Resources.Load<Sprite>(statelibrary + "number_7").texture; 
        textures[8] = Resources.Load<Sprite>(statelibrary + "number_8").texture;
        textures[9] = Resources.Load<Sprite>(statelibrary + "button.9").texture;
        textures[10] = Resources.Load<Sprite>(statelibrary + "flag").texture;
        textures[11] = Resources.Load<Sprite>(statelibrary + "bomb_normal").texture;
        textures[12] = Resources.Load<Sprite>(statelibrary + "bomb_exploded").texture;
    }
    public override void HighlightTiles(Vector2Int[] tiles, AIDataType aitype)
    {
        Vector2Int[] caitiles;

        //in case of invalid type there in only addition, so it doesn't need to be clared afterwords
        if (highlighttoupdate.ContainsKey(aitype) && aitype != AIDataType.Invalid)
        {
            caitiles = highlighttoupdate[aitype];
        }
        else
        {
            highlighttoupdate[aitype] = new Vector2Int[0];
            caitiles = highlighttoupdate[aitype];
        }
        //clearing previous texture
        for (int i = 0; i < caitiles.Length; i++)
        {
            highlighttmap.UpdateAt(caitiles[i], AIDataType.None);
        }

        //assign new tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            highlighttmap.UpdateAt(tiles[i], aitype);
        }
        highlighttoupdate[aitype] = tiles;
    }
    private void clearHighlightTiles() 
    {
        highlighttoupdate.Clear();
        AIDataType[] aitypes = new AIDataType[BoardSize.x * BoardSize.y];
        for (int i = 0; i < aitypes.Length; i++)
        {
            aitypes[i]= AIDataType.None;
        }
        highlighttmap.UpdateComplete(aitypes);
    }
    public override void SetUpVisualMinesweeper()
    {
        //setting up the texturemaps
        dict = new Dictionary<int, Color32[]>();
        dict.Add(0, textures[0].GetPixels32());
        dict.Add(1, textures[1].GetPixels32());
        dict.Add(2, textures[2].GetPixels32());
        dict.Add(3, textures[3].GetPixels32());
        dict.Add(4, textures[4].GetPixels32());
        dict.Add(5, textures[5].GetPixels32());
        dict.Add(6, textures[6].GetPixels32());
        dict.Add(7, textures[7].GetPixels32());
        dict.Add(8, textures[8].GetPixels32());
        dict.Add(9, textures[9].GetPixels32());
        dict.Add(10, textures[10].GetPixels32());
        dict.Add(11, textures[11].GetPixels32());
        dict.Add(12, textures[12].GetPixels32());
        textureMap = new TextureMap<int>(MinesweepergameImageParent.gameObject, highlightprefab, dict, BoardSize, TileSize, new Vector2Int(16, 16));

        Color[] highlightcolor = new Color[]
        {
                new Color(0, 0, 0, 0),
                new Color(1,0,0,0.3f),
                new Color(0,1,0,0.3f),
                new Color(0,0,1,0.3f),
                new Color(1,1,0,0.3f),
                new Color(0.2f,0.2f,0.2f,0.3f)
        };
        highlightcolors = new Dictionary<AIDataType, Color32[]>(10);
        highlightcolors.Add(AIDataType.None, new Color32[] { highlightcolor[0] });
        highlightcolors.Add(AIDataType.Mine, new Color32[] { highlightcolor[1] });
        highlightcolors.Add(AIDataType.Safe, new Color32[] { highlightcolor[2] });
        highlightcolors.Add(AIDataType.Open, new Color32[] { highlightcolor[3] });
        highlightcolors.Add(AIDataType.Closed, new Color32[] { highlightcolor[4] });
        highlightcolors.Add(AIDataType.Invalid, new Color32[] { highlightcolor[5] });
        highlighttoupdate = new Dictionary<AIDataType, Vector2Int[]>(10);
        highlighttmap = new TextureMap<AIDataType>(MinesweepergameImageParent, highlightprefab, highlightcolors, BoardSize, new Vector2Int(1, 1), BoardSize,  FilterMode.Point, true);
        Transform t = highlighttmap.GetChunkOfTile(0, 0).transform;
        t.localScale = (Vector2)TileSize;
        RectTransform rt = t.gameObject.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2();

        //setting the size of the parent object
        int x = BoardSize.x;
        int y = BoardSize.y;
        RectTransform rtp = MinesweepergameImageParent.gameObject.GetComponent<RectTransform>();
        rtp.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x * TileSize.x);
        rtp.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y * TileSize.y);
    }
    public override void UpdateVisualMinesweeperComplete(MinesweeperGamestate gamestate)
    {
        List<int> state = new List<int>(gamestate.LengthX * gamestate.LengthY);
        for (int y = 0; y < gamestate.LengthY; y++)
        {
            for (int x = 0; x < gamestate.LengthX; x++)
            {
                state.Add(GetSpriteId(gamestate[x, y]));
            }
        }
        textureMap.UpdateComplete(state);
        clearHighlightTiles();
    }
    public override void UpdateVisualMinesweeperQuick(KeyValuePair<Vector2Int, MinesweeperElementInfo>[] ToUpdate)
    {
        for (int i = 0; i < ToUpdate.Length; i++)
        {
            textureMap.UpdateAt(ToUpdate[i].Key, GetSpriteId(ToUpdate[i].Value));
        }
    }

    private int GetSpriteId(MinesweeperElementInfo msei)
    {
        if (msei.hidden)
        {
            if (msei.flaged)
            {
                return 10;
            }
            else
            {
                return 9;
            }
        }
        else
        {
            return (msei.value >= 9 ? 12 : msei.value);
        }
    }

    public override void FinalizeUpdate()
    {
        textureMap.UpdateFinalize();
        highlighttmap.UpdateFinalize();
    }
}

