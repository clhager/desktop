using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainModel : Model
{
    public static Dictionary<string, TerrainModel> Map = new Dictionary<string, TerrainModel>();

    private static Dictionary<Color32, Material> ColorToMaterial = new Dictionary<Color32, Material>();

    public string Tag { get; set;  }
    public Color32[] Colors { get; set;  }

    public TerrainModel(ReadMap.ObjectMap map) : base(map) { }

    public override void Init()
    {
        Map.Add(Tag, this);
        foreach (Color32 color in Colors)
        {
            if (!ColorToMaterial.ContainsKey(color))
            {
                Material newMaterial = new Material(Shader.Find("Unlit/Color"));
                newMaterial.color = color;
                ColorToMaterial.Add(color, newMaterial);
            }
        }
    }

    public static Color GetColor(HexModel hexModel)
    {
        TerrainModel terrainModel = Map[hexModel.TerrainType];
        Color32 color = terrainModel.Colors[Math.Mod(Mathf.RoundToInt(Random.value * terrainModel.Colors.Length), terrainModel.Colors.Length)];
        return color;
    }

    public class TerrainResourceModel : Model
    {
        public TerrainModel[] Terrains { get; set; }

        public TerrainResourceModel(ReadMap.ObjectMap map) : base(map) { }
    }
}
