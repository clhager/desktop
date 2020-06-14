using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverModel
{
    public static Dictionary<string, RiverModel> map = new Dictionary<string, RiverModel>();

    public string Name { get; set; }
    public Vector2Int[] Coordinates { get; set; }

    public RiverModel() {}

    public RiverModel(List<Vector2Int> coordinates)
    {
        Coordinates = coordinates.ToArray();
        map.Add(Name, this);
    }

    public (Vector2Int, HexGrid.Direction)[] GetBorderHexes()
    {
        List<(Vector2Int, HexGrid.Direction)> borderProvinces = new List<(Vector2Int, HexGrid.Direction)>();
        for (int i = 1; i < Coordinates.Length; i++)
        {
            Vector2Int start = Coordinates[i - 1];
            Vector2Int end = Coordinates[i];
            ((Vector2Int, HexGrid.Direction) pair1, (Vector2Int, HexGrid.Direction) pair2) = GetTwoBorderHexes(start, end);
            if (pair1.Item1.x >= 0 && pair1.Item1.y >= 0)
            {
                borderProvinces.Add(pair1);
            }
            if (pair2.Item1.x >= 0 && pair2.Item1.y >= 0)
            {
                borderProvinces.Add(pair2);
            }
        }
        return borderProvinces.ToArray();
    }

    private ((Vector2Int, HexGrid.Direction), (Vector2Int, HexGrid.Direction)) GetTwoBorderHexes(Vector2Int riverStart, Vector2Int riverEnd)
    {
        if (riverStart.x == riverEnd.x)
        {
            int x = riverStart.x / 2;
            int y = Mathf.Min(riverStart.y, riverEnd.y);
            return ((new Vector2Int(x - 1, y), HexGrid.Direction.East), (new Vector2Int(x, y), HexGrid.Direction.West));
        }

        else
        {
            int x = Mathf.Max(riverStart.x, riverEnd.x);
            int y = riverStart.y;
            if ((x + y).IsEven())
            {
                return ((new Vector2Int(x / 2 - 1, y), HexGrid.Direction.SouthEast), (new Vector2Int(x / 2, y - 1), HexGrid.Direction.NorthWest));
            }
            else
            {
                return ((new Vector2Int(x / 2 - 1, y), HexGrid.Direction.SouthWest), (new Vector2Int(x / 2 - 1, y - 1), HexGrid.Direction.NorthEast));
            }
        }
    }

    public bool VerifyCoordinatesAreAdjacent(Vector2Int[] coordinates)
    {
        for (int i = 1; i < coordinates.Length; i++)
        {
            if (!AreRiverCoordinatesAdjacent(coordinates[i - 1], coordinates[i])) return false;
        }
        return true;
    }

    private bool AreRiverCoordinatesAdjacent(Vector2Int first, Vector2Int next)
    {
        if (first.y == next.y && Mathf.Abs(first.x - next.x) <= 1) return true;
        if (first.x == next.x) {
            if ((first.x + first.y).IsEven() && first.y + 1 == next.y) return true;
            if (!(first.x + first.y).IsEven() && first.y - 1 == next.y) return true;
        }
        return false;
    }

    
}
