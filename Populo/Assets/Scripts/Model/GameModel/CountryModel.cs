using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryModel : Model
{
    public static Dictionary<string, CountryModel> Map = new Dictionary<string, CountryModel>();
    public static Dictionary<string, HashSet<HexModel>> CountryTagToHexes = new Dictionary<string, HashSet<HexModel>>();

    public string Tag { get; set; }
    public string Name { get; set; }
    public Color32 Color { get; set; }

    public CountryModel(ReadMap.ObjectMap map) : base(map) { }
}
