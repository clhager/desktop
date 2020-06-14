using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*new HexModel(new Vector2Int(0, 0), 1, new Terrain());
        new HexModel(new Vector2Int(1, 0), 1, new Terrain());
        new HexModel(new Vector2Int(0, 1), 1, new Terrain());
        new HexModel(new Vector2Int(1, 1), 1, new Terrain());
        new MapModel("defaultTerrain");
        GameFolder.SetupMainDirectory();
        Write.WriteObjectToFile(GameFolder.MapsDirectory, "BasicMap", MapModel.map);*/

        Read.ReadResource("Resources", "Terrain", typeof(TerrainModel.TerrainResourceModel));
        Read.ReadMapFromFile("Maps", "SmallMap");
        MapModel.FillMissingHexes();
        FlatHexMap.RenderAll();
        FlatHexMap.SetMapColors(new MapMode.ElevationMapMode().GetMapColors());
        /*
        int width = 4;
        int height = 4;
        HexGrid grid = new HexGrid(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid.Add(new HexModel(new Coordinates(x, y), 1, new HexGrid.Direction[] { }));
            }
        }

        grid.Get(new Coordinates(1, 2)).unconnectedNeighbors = new HexGrid.Direction[] { HexGrid.Direction.East };
        grid.GetNeighbor(new Coordinates(1, 2), HexGrid.Direction.East).unconnectedNeighbors = new HexGrid.Direction[] { HexGrid.Direction.West };
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
