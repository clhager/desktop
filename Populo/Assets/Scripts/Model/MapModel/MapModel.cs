using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModel : Model
{
    public static MapModel map = null;

    public string DefaultTerrain { get; set; }
    public HexModel[] Hexes { get { return Grid.GridToOrderedArray(HexModel.map); } set { } }

    public static int GetWidth()
    {
        return HexModel.width;
    }

    public static int GetHeight()
    {
        return HexModel.height;
    }

    public MapModel(ReadMap.ObjectMap map) : base(map) { }

    public MapModel(string defaultTerrain) : base()
    {
        DefaultTerrain = defaultTerrain;
    }

    public override void Init()
    {
        if (map != null) throw new ModelException("Cannot have more than one MapModel");
        map = this;
    }

    public static void FillMissingHexes()
    {
        for (int i = 0; i < GetWidth(); i++)
        {
            for (int j = 0; j < GetHeight(); j++)
            {
                if (!HexModel.map.ContainsKey(new Vector2Int(i, j)))
                {
                    new HexModel(new Vector2Int(i, j), 0, map.DefaultTerrain);
                }
            }
        }
    }

}
