using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameFolder
{
    public static readonly string MainDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Populo");
    public static readonly string MapsDirectory = Path.Combine(MainDirectory, "Maps");
    public static readonly string ModsDirectory = Path.Combine(MainDirectory, "Mods");

    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static void SetupMainDirectory()
    {
        CreateDirectory(MainDirectory);
        CreateDirectory(MapsDirectory);
        CreateDirectory(ModsDirectory);
    }


}
