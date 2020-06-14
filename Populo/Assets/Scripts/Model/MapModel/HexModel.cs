using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexModel : Model
{
    public static Dictionary<Vector2Int, HexModel> map = new Dictionary<Vector2Int, HexModel>();
    public static int width = 0;
    public static int height = 0;

    public Vector2Int Coordinates { get; set; }
    public int Elevation { get; set; }
    public string TerrainType { get; set; }
    public string CountryOwnership { get; set; }

    public HexModel(ReadMap.ObjectMap map) : base(map) { }

    public HexModel(
        Vector2Int coordinates,
        int elevation,
        string terrain
    )
    {
        Coordinates = coordinates;
        Elevation = elevation;
        TerrainType = terrain;
        Init();
    }

    public override void Init()
    {
        width = Mathf.Max(width, Coordinates.x + 1);
        height = Mathf.Max(height, Coordinates.y + 1);
        if (map.ContainsKey(Coordinates)) throw new ModelException($"Duplicate HexModel found for coordinates {Coordinates}");
        map.Add(Coordinates, this);
    }

}
