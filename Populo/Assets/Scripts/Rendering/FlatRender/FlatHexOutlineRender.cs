using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatHexOutlineRender
{
    public enum Side { Inside, Outside }

    public static Mesh RenderHexSingleSideOutline(HexModel hexModel, HexVector.Direction direction, Side renderSide, bool includeLeft, bool includeRight, float thickness)
    {
        List<Vector3> vertices;
        List<(int, int, int)> triangles;

        if (renderSide == Side.Inside) (vertices, triangles) = RenderHexSingleSideOutlineInside(hexModel, direction, includeLeft, includeRight, thickness);
        else (vertices, triangles) = RenderHexSingleSideOutlineInside(hexModel, direction, includeLeft, includeRight, thickness);

        Mesh mesh = new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = triangles.Flatten().ToArray()
        };
        mesh.RecalculateNormals();
        return mesh;
    }

    private static (List<Vector3>, List<(int, int, int)>) RenderHexSingleSideOutlineInside(HexModel hexModel, HexVector.Direction direction, bool includeLeft, bool includeRight, float thickness)
    {
        Vector2 leftVector = HexVector.DirectionToLeftVector[direction];
        Vector2 rightVector = HexVector.Clockwise.GetNext(leftVector);
        Vector2 leftOfLeftVector = HexVector.Clockwise.GetPrevious(leftVector);
        Vector2 rightOfRightVector = HexVector.Clockwise.GetNext(rightVector);

        List<Vector3> vertices = new List<Vector3>();
        Vector3 centerVertex = FlatHexRender.GetCenterVertex();

        int nearLeftVertex;
        if (includeLeft) nearLeftVertex = vertices.AddThenGetIndex(centerVertex + (leftOfLeftVector * thickness).Vector3(0) + (leftVector * (HexRender.HexSideLength - thickness)).Vector3(0));
        else nearLeftVertex = vertices.AddThenGetIndex(centerVertex + (leftVector * (HexRender.HexSideLength - thickness)).Vector3(0));
        int nearRightVertex;
        if (includeRight) nearRightVertex = vertices.AddThenGetIndex(centerVertex + (rightOfRightVector * thickness).Vector3(0) + (rightVector * (HexRender.HexSideLength - thickness)).Vector3(0));
        else nearRightVertex = vertices.AddThenGetIndex(centerVertex + (rightVector * (HexRender.HexSideLength - thickness)).Vector3(0));

        int farLeftVertex = vertices.AddThenGetIndex(centerVertex + (leftVector * HexRender.HexSideLength).Vector3(0));
        int farRightVertex = vertices.AddThenGetIndex(centerVertex + (rightVector * HexRender.HexSideLength).Vector3(0));

        (int, int, int) nearTriangle = (nearLeftVertex, farLeftVertex, nearRightVertex);
        (int, int, int) farTriangle = (nearRightVertex, farLeftVertex, farRightVertex);
        List<(int, int, int)> triangles = new List<(int, int, int)> { nearTriangle, farTriangle };

        return (vertices, triangles);
    }

    private static (List<Vector3>, List<(int, int, int)>) RenderHexSingleSideOutlineOutside(HexModel hexModel, HexVector.Direction direction, bool includeLeft, bool includeRight, float thickness)
    {
        Vector2 leftVector = HexVector.DirectionToLeftVector[direction];
        Vector2 rightVector = HexVector.Clockwise.GetNext(leftVector);

        List<Vector3> vertices = new List<Vector3>();
        Vector3 centerVertex = FlatHexRender.GetCenterVertex();

        int farLeftVertex;
        if (includeLeft) farLeftVertex = vertices.AddThenGetIndex(centerVertex + (leftVector * (HexRender.HexSideLength + thickness)).Vector3(0));
        else farLeftVertex = vertices.AddThenGetIndex(centerVertex + (rightVector * thickness).Vector3(0) + (leftVector * (HexRender.HexSideLength - thickness)).Vector3(0));

        int farRightVertex;
        if (includeLeft) farRightVertex = vertices.AddThenGetIndex(centerVertex + (rightVector * (HexRender.HexSideLength + thickness)).Vector3(0));
        else farRightVertex = vertices.AddThenGetIndex(centerVertex + (leftVector * thickness).Vector3(0) + (rightVector * (HexRender.HexSideLength - thickness)).Vector3(0));

        int nearLeftVertex = vertices.AddThenGetIndex(centerVertex + (leftVector * HexRender.HexSideLength).Vector3(0));
        int nearRightVertex = vertices.AddThenGetIndex(centerVertex + (rightVector * HexRender.HexSideLength).Vector3(0));

        (int, int, int) nearTriangle = (nearLeftVertex, farLeftVertex, nearRightVertex);
        (int, int, int) farTriangle = (nearRightVertex, farLeftVertex, farRightVertex);
        List<(int, int, int)> triangles = new List<(int, int, int)> { nearTriangle, farTriangle };

        return (vertices, triangles);
    }
}
