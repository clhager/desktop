using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexGrid
{
    public enum Direction
    {
        NorthEast, East, SouthEast, SouthWest, West, NorthWest
    }
    public static readonly ModularOrderedSet<Direction> Directions = new ModularOrderedSet<Direction>(
        new Direction[] { Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.SouthWest, Direction.West, Direction.NorthWest }    
    );

    public int width;
    public int height;
    public HexModel[,] hexModels;

    public HexGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        hexModels = new HexModel[width, height];
    }

    public HexModel Get(Vector2Int coordinates)
    {
        if (coordinates.x >= width || coordinates.x < 0 || coordinates.y >= height || coordinates.y < 0)
        {
            return null;
        }

        return hexModels[coordinates.x, coordinates.y];
    }

    public void Add(HexModel hexModel)
    {
        hexModels[hexModel.Coordinates.x, hexModel.Coordinates.y] = hexModel;
    }

    public void Set(Vector2Int coordinates, HexModel hexModel)
    {
        hexModels[coordinates.x, coordinates.y] = hexModel;
    }

    public HexModel GetNeighbor(Vector2Int coordinates, Direction direction)
    {
        return Get(coordinates + GetCoordinatesFromDirection(coordinates, direction));
    }

    public static HexModel GetNeighbor(HexModel hexModel, HexVector.Direction direction)
    {
        return HexModel.map[hexModel.Coordinates + GetCoordinatesFromDirection(hexModel.Coordinates, direction)];
    }

    public HexModel[] GetNeighbors(Vector2Int coordinates)
    {
        return (from direction in Directions.GetEnumerable() select GetNeighbor(coordinates, direction)).ToArray();
    }

    public IEnumerable<Direction> GetDirections()
    {
        return Directions.GetEnumerable();
    }

    private static Vector2Int GetCoordinatesFromDirection(Vector2Int coordinates, Direction direction)
    {
        bool evenRow = coordinates.y % 2 == 0;
        int x = 0, y = 0;
        switch (direction)
        {
            case Direction.NorthEast:
                if (evenRow) (x, y) = (0, 1);
                else (x, y) = (1, 1);
                break;
            case Direction.East:
                (x, y) = (1, 0);
                break;
            case Direction.SouthEast:
                if (evenRow) (x, y) = (0, -1);
                else (x, y) = (1, -1);
                break;
            case Direction.SouthWest:
                if (evenRow) (x, y) = (-1, -1);
                else (x, y) = (0, -1);
                break;
            case Direction.West:
                 (x, y) = (-1, 0);
                break;
            case Direction.NorthWest:
                if (evenRow) (x, y) = (-1, 1);
                else (x, y) = (0, 1);
                break;
        }

        return new Vector2Int(x, y);
    }

    private static Vector2Int GetCoordinatesFromDirection(Vector2Int coordinates, HexVector.Direction direction)
    {
        bool evenRow = coordinates.y % 2 == 0;
        int x = 0, y = 0;
        switch (direction)
        {
            case HexVector.Direction.NorthEast:
                if (evenRow) (x, y) = (0, 1);
                else (x, y) = (1, 1);
                break;
            case HexVector.Direction.East:
                (x, y) = (1, 0);
                break;
            case HexVector.Direction.SouthEast:
                if (evenRow) (x, y) = (0, -1);
                else (x, y) = (1, -1);
                break;
            case HexVector.Direction.SouthWest:
                if (evenRow) (x, y) = (-1, -1);
                else (x, y) = (0, -1);
                break;
            case HexVector.Direction.West:
                (x, y) = (-1, 0);
                break;
            case HexVector.Direction.NorthWest:
                if (evenRow) (x, y) = (-1, 1);
                else (x, y) = (0, 1);
                break;
        }

        return new Vector2Int(x, y);
    }

    public static Vector2 CoordinatesToHexGridPosition(Vector2Int coordinates)
    {
        float xOffset = Mathf.Sqrt(3) * HexRender.HexSideLength * (1.0f / 2.0f);
        float x = coordinates.x * xOffset * 2;
        float y = coordinates.y * 1.5f * HexRender.HexSideLength;
        if (y % 2 == 1)
        {
            x += xOffset;
        }
        return new Vector2(x, y);
    }
}
