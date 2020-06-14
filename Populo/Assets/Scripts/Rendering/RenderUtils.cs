using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderUtils
{
    public static (List<Vector3> vertices, List<(int, int, int)> triangles) MergeVerticesAndTriangles(
        (List<Vector3> vertices, List<(int, int, int)> triangles) mainList,
        (List<Vector3> vertices, List<(int, int, int)> triangles) subList
    )
    {
        int vertexBaseline = mainList.vertices.Count;
        mainList.vertices.AddRange(subList.vertices);
        foreach ((int, int, int) triangle in subList.triangles)
        {
            mainList.triangles.Add((triangle.Item1 + vertexBaseline, triangle.Item2 + vertexBaseline, triangle.Item3 + vertexBaseline));
        }

        return (mainList.vertices, mainList.triangles);
    }
}
