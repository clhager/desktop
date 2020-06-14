using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatHexRender : MonoBehaviour
{
    public static Mesh RenderHex(HexModel hexModel)
    {
        List<Vector3> vertices = new List<Vector3>();
        Vector3 centerVertex = GetCenterVertex();
        int northVertex = vertices.AddThenGetIndex(centerVertex + HexConfig.HexSideLength * HexVector.North.Vector3(centerVertex.y));
        int northEastVertex = vertices.AddThenGetIndex(centerVertex + HexConfig.HexSideLength * HexVector.NorthEast.Vector3(centerVertex.y));
        int southEastVertex = vertices.AddThenGetIndex(centerVertex + HexConfig.HexSideLength * HexVector.SouthEast.Vector3(centerVertex.y));
        int southVertex = vertices.AddThenGetIndex(centerVertex + HexConfig.HexSideLength * HexVector.South.Vector3(centerVertex.y));
        int southWestVertex = vertices.AddThenGetIndex(centerVertex + HexConfig.HexSideLength * HexVector.SouthWest.Vector3(centerVertex.y));
        int northWestVertex = vertices.AddThenGetIndex(centerVertex + HexConfig.HexSideLength * HexVector.NorthWest.Vector3(centerVertex.y));

        (int, int, int) topTriangle = (northWestVertex, northVertex, northEastVertex);
        (int, int, int) topMiddleTriangle = (southWestVertex, northWestVertex, northEastVertex);
        (int, int, int) bottomMiddleTriangle = (southWestVertex, northEastVertex, southEastVertex);
        (int, int, int) bottomTriangle = (southWestVertex, southEastVertex, southVertex);
        List<(int, int, int)> triangles = new List<(int, int, int)> { topTriangle, topMiddleTriangle, bottomMiddleTriangle, bottomTriangle };

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = FlattenTriangleList(triangles).ToArray(),
            colors = HexColorCoordinates.GetColorCoordinates(hexModel.Coordinates).ListOfN(vertices.Count).ToArray()
        };
        mesh.RecalculateNormals();
        return mesh;

        List<int> FlattenTriangleList(List<(int, int, int)> triangleList)
        {
            List<int> flattenedTriangles = new List<int>();
            foreach ((int, int, int) triangle in triangleList)
            {
                flattenedTriangles.Add(triangle.Item1);
                flattenedTriangles.Add(triangle.Item2);
                flattenedTriangles.Add(triangle.Item3);
            }
            return flattenedTriangles;
        }
    }

    public static Vector3 GetCenterVertex()
    {
        return new Vector3();
    }
}
