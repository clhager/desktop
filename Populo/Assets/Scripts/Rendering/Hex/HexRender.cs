using System.Collections.Generic;
using UnityEngine;

public class HexRender
{
    public static readonly float HexSideLength = 10.0f;
    public static readonly int HexTransitionSteps = 2;
    public static readonly bool GiveMiddleToUpperHex = true;

    public static readonly float StepRampWidth = 0.5f;
    public static readonly float StepFlatWidth = 1.0f;
    public static readonly float StepHeight = 1.0f;
    public static readonly float StepWidth = StepRampWidth + StepFlatWidth;

    public static readonly float ElevationUnit = StepHeight * (HexTransitionSteps + 1);

    private static readonly Vector2 North = new Vector2(0.0f, 1.0f);
    private static readonly Vector2 NorthEast = new Vector2(Mathf.Sqrt(3) / 2.0f, 0.5f);
    private static readonly Vector2 NorthWest = new Vector2(-Mathf.Sqrt(3) / 2.0f, 0.5f);
    private static readonly Vector2 South = new Vector2(0.0f, -1.0f);
    private static readonly Vector2 SouthEast = new Vector2(Mathf.Sqrt(3) / 2.0f, -0.5f);
    private static readonly Vector2 SouthWest = new Vector2(-Mathf.Sqrt(3) / 2.0f, -0.5f);
    private static readonly ModularOrderedSet<Vector2> Clockwise = new ModularOrderedSet<Vector2>(new Vector2[] { North, NorthEast, SouthEast, South, SouthWest, NorthWest });
    private static readonly Dictionary<HexGrid.Direction, Vector2> DirectionToLeftVector = new Dictionary<HexGrid.Direction, Vector2> {
        { HexGrid.Direction.NorthEast, North }, { HexGrid.Direction.East, NorthEast }, { HexGrid.Direction.SouthEast, SouthEast},
        { HexGrid.Direction.SouthWest, South }, { HexGrid.Direction.West, SouthWest }, { HexGrid.Direction.NorthWest, NorthWest }
    };

    public static Mesh RenderHex(HexGrid grid, Vector2Int hexCoordinates)
    {
        List<Vector3> meshVertices = new List<Vector3>();
        List<(int, int, int)> meshTriangles = new List<(int, int, int)>();

        foreach (HexGrid.Direction direction in HexGrid.Directions.GetEnumerable())
        {
            (meshVertices, meshTriangles) = RenderUtils.MergeVerticesAndTriangles((meshVertices, meshTriangles), RenderHexTriangle(grid, hexCoordinates, direction));
        }

        Mesh mesh = new Mesh();
        mesh.vertices = meshVertices.ToArray();
        mesh.triangles = FlattenTriangleList(meshTriangles).ToArray();
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

    private static (List<Vector3> vertices, List<(int, int, int)> triangles) RenderHexTriangle(HexGrid grid, Vector2Int hexCoordinates, HexGrid.Direction direction)
    {
        HexModel currentHex = grid.Get(hexCoordinates);
        HexModel neighborHex = grid.GetNeighbor(hexCoordinates, direction);
        HexModel leftNeighborHex = grid.GetNeighbor(hexCoordinates, HexGrid.Directions.GetPrevious(direction));
        HexModel rightNeighborHex = grid.GetNeighbor(hexCoordinates, HexGrid.Directions.GetNext(direction));

        if (neighborHex == null || currentHex.Elevation == neighborHex.Elevation)
        {
            return RenderEvenWithNeighborHexTriangle(grid, hexCoordinates, direction);
        }

        float offsetHeight = GetOffsetHeight(currentHex.Elevation, neighborHex.Elevation);
        Vector2 leftVector = DirectionToLeftVector[direction];
        Vector2 rightVector = Clockwise.GetNext(leftVector);
        Vector2 leftOfLeftVector = Clockwise.GetPrevious(leftVector);
        Vector2 rightOfRightVector = Clockwise.GetNext(rightVector);

        (List<Vector3> vertices, List<(int, int, int)> triangles) = (new List<Vector3>(), new List<(int, int, int)>());
        (vertices, triangles) = RenderUtils.MergeVerticesAndTriangles(
            (vertices, triangles),
            RenderTriangle(GetCenterVertex(), leftVector, rightVector, GetEdgeOfBaseTriangle(), GetEdgeOfBaseTriangle())
        );

        Vector3 leftStart;
        Vector3 leftStepsVector;
        if (leftNeighborHex == null || currentHex.Elevation <= leftNeighborHex.Elevation && leftNeighborHex.Elevation != neighborHex.Elevation)
        {
            leftStart = (GetCenterVertex() + leftVector * GetEdgeOfBaseTriangle() + leftOfLeftVector * (HexSideLength - GetEdgeOfBaseTriangle())).Vector3(offsetHeight);
            leftStepsVector = rightVector;
        }
        else
        {
            leftStart = (GetCenterVertex() + leftVector * GetEdgeOfBaseTriangle()).Vector3(offsetHeight);
            leftStepsVector = leftVector;
        }
        Vector3 rightStart;
        Vector3 rightStepsVector;
        if (rightNeighborHex == null || currentHex.Elevation <= rightNeighborHex.Elevation && rightNeighborHex.Elevation != neighborHex.Elevation)
        {
            rightStart = (GetCenterVertex() + rightVector * GetEdgeOfBaseTriangle() + rightOfRightVector * (HexSideLength - GetEdgeOfBaseTriangle())).Vector3(offsetHeight);
            rightStepsVector = leftVector;
        }
        else
        {
            rightStart = (GetCenterVertex() + rightVector * GetEdgeOfBaseTriangle()).Vector3(offsetHeight);
            rightStepsVector = rightVector;
        }

        (vertices, triangles) = RenderUtils.MergeVerticesAndTriangles(
            (vertices, triangles),
            RenderSteps(
                leftStart, rightStart, leftStepsVector, rightStepsVector, GetStepHeight(currentHex.Elevation, neighborHex.Elevation), 
                HexTransitionSteps / 2, GetIncludeExtraRamp(currentHex.Elevation, neighborHex.Elevation)
            )
        );

        return (vertices, triangles);

        float GetOffsetHeight(int currentHexElevation, int neighborHexElevation)
        {
            if (currentHexElevation < neighborHexElevation)
            {
                return 0;
            }
            else
            {
                return -ElevationUnit * (currentHexElevation - neighborHexElevation - 1);
            }
        }

        float GetStepHeight(int currentHexElevation, int neighborHexElevation)
        {
            if (currentHexElevation < neighborHexElevation)
            {
                return StepHeight;
            }
            else
            {
                return -StepHeight;
            }
        }

        bool GetIncludeExtraRamp(int currentHexElevation, int neighborHexElevation)
        {
            if (currentHexElevation > neighborHexElevation && GiveMiddleToUpperHex)
            {
                return true;
            }
            if (currentHexElevation < neighborHexElevation && !GiveMiddleToUpperHex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private static (List<Vector3> vertices, List<(int, int, int)> triangles) RenderEvenWithNeighborHexTriangle(HexGrid grid, Vector2Int hexCoordinates, HexGrid.Direction direction)
    {
        (List<Vector3> vertices, List<(int, int, int)> triangles) = (new List<Vector3>(), new List<(int, int, int)>());

        HexModel currentHex = grid.Get(hexCoordinates);
        HexModel leftNeighborHex = grid.GetNeighbor(hexCoordinates, HexGrid.Directions.GetPrevious(direction));
        HexModel rightNeighborHex = grid.GetNeighbor(hexCoordinates, HexGrid.Directions.GetNext(direction));
        Vector2 leftVector = DirectionToLeftVector[direction];
        Vector2 rightVector = Clockwise.GetNext(leftVector);
        Vector3 centerVertex = GetCenterVertex();

        float vectorRatio = GetEdgeOfBaseTriangle() / HexSideLength;

        int centerVertexId = vertices.AddThenGetIndex(GetCenterVertex());
        int leftCenterVertexId;
        int rightCenterVertexId;

        if (leftNeighborHex == null || currentHex.Elevation == leftNeighborHex.Elevation)
        {
            leftCenterVertexId = vertices.AddThenGetIndex(centerVertex + (leftVector * HexSideLength).Vector3(0));
        }
        else
        {
            int leftVertexId = vertices.AddThenGetIndex(centerVertex + (leftVector * (vectorRatio * HexSideLength)).Vector3(0));
            leftCenterVertexId = vertices.AddThenGetIndex(vertices[leftVertexId] + (rightVector * ((1 - vectorRatio) * HexSideLength)).Vector3(0));
            (int, int, int) leftTriangle = (centerVertexId, leftVertexId, leftCenterVertexId);
            triangles.Add(leftTriangle);
        }

        if (rightNeighborHex == null || currentHex.Elevation == rightNeighborHex.Elevation)
        {
            rightCenterVertexId = vertices.AddThenGetIndex(centerVertex + (rightVector * HexSideLength).Vector3(0));
        }
        else
        {
            int rightVertexId = vertices.AddThenGetIndex(centerVertex + (rightVector * (vectorRatio * HexSideLength)).Vector3(0));
            rightCenterVertexId = vertices.AddThenGetIndex(vertices[rightVertexId] + (leftVector * ((1 - vectorRatio) * HexSideLength)).Vector3(0));
            (int, int, int) rightTriangle = (centerVertexId, rightCenterVertexId, rightVertexId);
            triangles.Add(rightTriangle);
        }
        (int, int, int) centerTriangle = (centerVertexId, leftCenterVertexId, rightCenterVertexId);
        triangles.Add(centerTriangle);

        return (vertices, triangles);
    }

    private static (List<Vector3> vertices, List<(int, int, int)> triangles) RenderSteps(Vector3 leftStart, Vector3 rightStart, Vector2 leftVector, Vector2 rightVector, float stepHeight, int numSteps, bool includeExtraRamp)
    {
        (List<Vector3> vertices, List<(int, int, int)> triangles) = (new List<Vector3>(), new List<(int, int, int)>());

        for (int stepIndex = 0; stepIndex < numSteps; stepIndex++)
        {
            Vector3 leftEnd = leftStart + (leftVector * StepRampWidth).Vector3(stepHeight * (stepIndex + 1));
            Vector3 rightEnd = rightStart + (rightVector * StepRampWidth).Vector3(stepHeight * (stepIndex + 1));

            (vertices, triangles) = RenderUtils.MergeVerticesAndTriangles((vertices, triangles), RenderQuad(leftStart, rightStart, leftEnd, rightEnd));

            leftStart = leftEnd;
            rightStart = rightEnd;
            leftEnd = leftStart + (leftVector * StepFlatWidth).Vector3(0);
            rightEnd = rightStart + (rightVector * StepFlatWidth).Vector3(0);

            (vertices, triangles) = RenderUtils.MergeVerticesAndTriangles((vertices, triangles), RenderQuad(leftStart, rightStart, leftEnd, rightEnd));

            leftStart = leftEnd;
            rightStart = rightEnd;
        }

        if (includeExtraRamp)
        {
            Vector3 leftEnd = leftStart + (leftVector * StepRampWidth).Vector3(stepHeight * numSteps);
            Vector3 rightEnd = rightStart + (rightVector * StepRampWidth).Vector3(stepHeight * numSteps);

            (vertices, triangles) = RenderUtils.MergeVerticesAndTriangles((vertices, triangles), RenderQuad(leftStart, rightStart, leftEnd, rightEnd));
        }

        return (vertices, triangles);
    }

    private static (List<Vector3> vertices, List<(int, int, int)> triangles) RenderQuad(Vector3 leftStart, Vector3 rightStart, Vector3 leftEnd, Vector3 rightEnd)
    {
        List<Vector3> vertices = new List<Vector3>();
        int leftStartId = vertices.AddThenGetIndex(leftStart);
        int rightStartId = vertices.AddThenGetIndex(rightStart);
        int leftEndId = vertices.AddThenGetIndex(leftEnd);
        int rightEndId = vertices.AddThenGetIndex(rightEnd);

        (int, int, int) triangle1 = (leftStartId, leftEndId, rightStartId);
        (int, int, int) triangle2 = (rightStartId, leftEndId, rightEndId);
        List<(int, int, int)> triangles = new List<(int, int, int)> { triangle1, triangle2 };

        return (vertices, triangles);
    }

    private static (List<Vector3> vertices, List<(int, int, int)> triangles) RenderTriangle(Vector3 centerVertex, Vector2 leftVector, Vector2 rightVector, float leftSideLength, float rightSideLength)
    {
        List<Vector3> vertices = new List<Vector3>();
        int centerVertexId = vertices.AddThenGetIndex(centerVertex);
        int leftVertexId = vertices.AddThenGetIndex(centerVertex + (leftVector * leftSideLength).Vector3(centerVertex.y));
        int rightVertexId = vertices.AddThenGetIndex(centerVertex + (rightVector * rightSideLength).Vector3(centerVertex.y));

        (int, int, int) triangle = (centerVertexId, leftVertexId, rightVertexId);
        List<(int, int, int)> triangles = new List<(int, int, int)> { triangle };

        return (vertices, triangles);
    }

    private static float GetEdgeOfBaseTriangle()
    {
        float edgeOfBaseTriangle = HexSideLength - StepWidth * (HexTransitionSteps / 2);
        if (HexTransitionSteps.IsEven())
        {
            edgeOfBaseTriangle -= StepRampWidth / 2.0f;
        }
        else
        {
            edgeOfBaseTriangle -= StepRampWidth;
            edgeOfBaseTriangle -= StepFlatWidth / 2.0f;
        }
        return edgeOfBaseTriangle;
    }

    private static Vector2 GetCenterVertex()
    {
        return new Vector2();
    }
}
