using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatHexMap : MonoBehaviour
{
    private static GameObject FlatMap = new GameObject("FlatMap");
    private static GameObject FlatHexHolder = new GameObject("FlatHexes").Apply((GameObject o) => o.transform.parent = FlatMap.transform);
    private static GameObject FlatHexShadowHolder = new GameObject("FlatHexShadows").Apply((GameObject o) => o.transform.parent = FlatMap.transform);
    private static GameObject[,] FlatHexes;
    private static Material VertexColorIndexerMaterial = new Material(Resources.Load<Shader>("Shaders/VertexColorIndexer"));
    private static Material ShadowMaterial = new Material(Resources.Load<Shader>("Shaders/UnlitShadow"));
    private static float ElevationShadowThickness = 2.0f;
    private static float MaxElevationShadowThickness = 6.0f;

    public Texture2D tex;

    public static void Render(HexModel hex)
    {
        if (Get(hex.Coordinates.x, hex.Coordinates.y) == null)
        {
            GameObject hexObject = CreateHexObject();
            Set(hex.Coordinates.x, hex.Coordinates.y, hexObject);
            
        }
        Get(hex.Coordinates.x, hex.Coordinates.y).GetComponent<MeshFilter>().mesh = FlatHexRender.RenderHex(hex);
        return;

        GameObject CreateHexObject()
        {
            GameObject hexObject = new GameObject($"FlatHex@{hex.Coordinates}").Apply((GameObject o) => o.transform.parent = FlatHexHolder.transform);
            hexObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = hexObject.AddComponent<MeshRenderer>();
            meshRenderer.material = VertexColorIndexerMaterial;
            hexObject.transform.position += HexGrid.CoordinatesToHexGridPosition(hex.Coordinates).Vector3(0);
            return hexObject;
        }

        GameObject CreateShadowObjects()
        {
            GameObject shadowObject = new GameObject($"FlatHexShadow@{hex.Coordinates}").Apply((GameObject o) => o.transform.parent = FlatHexShadowHolder.transform);
            shadowObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = shadowObject.AddComponent<MeshRenderer>();
            meshRenderer.material = VertexColorIndexerMaterial;
            shadowObject.transform.position += HexGrid.CoordinatesToHexGridPosition(hex.Coordinates).Vector3(0);

            Mesh mesh = new Mesh();
            foreach (HexVector.Direction direction in HexVector.Directions)
            {
                int elevationDiff = hex.Elevation - HexGrid.GetNeighbor(hex, direction).Elevation;
                if (elevationDiff > 0)
                {
                    bool includeLeft = false, includeRight = false;
                    if (hex.Elevation > HexGrid.GetNeighbor(hex, HexVector.Directions.GetPrevious(direction)).Elevation) includeLeft = true;
                    if (hex.Elevation > HexGrid.GetNeighbor(hex, HexVector.Directions.GetNext(direction)).Elevation) includeRight = true;
                    MeshExtensions.CombineMeshes(mesh, FlatHexOutlineRender.RenderHexSingleSideOutline(
                        hex,
                        direction,
                        FlatHexOutlineRender.Side.Outside,
                        includeLeft,
                        includeRight,
                        Mathf.Max(elevationDiff * ElevationShadowThickness, MaxElevationShadowThickness)
                    ));
                }   
            }
            return shadowObject;
        }
    }

    public static void RenderAll()
    {
        foreach (HexModel hex in MapModel.map.Hexes) Render(hex);
    }

    public static void SetMapColors(Color[,] colors)
    {
        if (colors.GetLength(0) != MapModel.GetWidth() || colors.GetLength(1) != MapModel.GetHeight())
        {
            throw new RenderException("color array does not match the dimensions of the map");
        }
        Texture2D mapTexture = CreateTexture(colors);

        VertexColorIndexerMaterial.SetTexture("_MainTex", mapTexture);
        VertexColorIndexerMaterial.mainTexture = mapTexture;
        VertexColorIndexerMaterial.SetInt("_Width", MapModel.GetWidth());
        VertexColorIndexerMaterial.SetInt("_Height", MapModel.GetHeight());
        //ItemHolder.AddComponent<FlatHexMap>().tex = mapTexture;
    }

    private static Texture2D CreateTexture(Color[,] colors)
    {
        Texture2D mapTexture = new Texture2D(MapModel.GetWidth(), MapModel.GetHeight());
        Color[] colorArray = new Color[MapModel.GetWidth() * MapModel.GetHeight()];
        for (int i = 0; i < colors.GetLength(0); i++)
        {
            for (int j = 0; j < colors.GetLength(1); j++)
            {
                colorArray[i * MapModel.GetHeight() + j] = colors[i, j];
            }
        }
        mapTexture.filterMode = FilterMode.Point;
        mapTexture.SetPixels(colorArray);
        mapTexture.Apply();
        return mapTexture;
    }

    private static GameObject Get(int x, int y)
    {
        if (FlatHexes == null)
        {
            FlatHexes = new GameObject[MapModel.GetWidth(), MapModel.GetHeight()];
        }
        return FlatHexes[x, y];
    }

    private static void Set(int x, int y, GameObject gameObject)
    {
        if (FlatHexes == null)
        {
            FlatHexes = new GameObject[MapModel.GetWidth(), MapModel.GetHeight()];
        }
        FlatHexes[x, y] = gameObject;
    }



    public class RenderException : UnityException
    {
        public RenderException(string message) : base(message) { }
    }
}
