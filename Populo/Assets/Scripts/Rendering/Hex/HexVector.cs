using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexVector
{
    public enum Direction
    {
        NorthEast, East, SouthEast, SouthWest, West, NorthWest
    }
    public static readonly ModularOrderedSet<Direction> Directions = new ModularOrderedSet<Direction>(
        new Direction[] { Direction.NorthEast, Direction.East, Direction.SouthEast, Direction.SouthWest, Direction.West, Direction.NorthWest }
    );

    public static readonly Vector2 North = new Vector2(0.0f, 1.0f);
    public static readonly Vector2 NorthEast = new Vector2(Mathf.Sqrt(3) / 2.0f, 0.5f);
    public static readonly Vector2 NorthWest = new Vector2(-Mathf.Sqrt(3) / 2.0f, 0.5f);
    public static readonly Vector2 South = new Vector2(0.0f, -1.0f);
    public static readonly Vector2 SouthEast = new Vector2(Mathf.Sqrt(3) / 2.0f, -0.5f);
    public static readonly Vector2 SouthWest = new Vector2(-Mathf.Sqrt(3) / 2.0f, -0.5f);
    public static readonly ModularOrderedSet<Vector2> Clockwise = new ModularOrderedSet<Vector2>(new Vector2[] { North, NorthEast, SouthEast, South, SouthWest, NorthWest });
    public static readonly Dictionary<Direction, Vector2> DirectionToLeftVector = new Dictionary<Direction, Vector2> {
        { Direction.NorthEast, North }, { Direction.East, NorthEast }, { Direction.SouthEast, SouthEast},
        { Direction.SouthWest, South }, { Direction.West, SouthWest }, { Direction.NorthWest, NorthWest }
    };
}
  