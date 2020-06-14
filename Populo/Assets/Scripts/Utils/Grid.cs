using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Grid
{
    
    public static T[] GridToOrderedArray<T>(Dictionary<Vector2Int, T> grid)
    {
        List<Vector2Int> coordinates = new List<Vector2Int> (grid.Keys);
        coordinates.Sort((Vector2Int first, Vector2Int second) => {
            if (first.x == second.x) return first.y - second.y;
            else return first.x - second.x;
        });
        return coordinates.Select((coordinate) => grid[coordinate]).ToArray();
    }

}
