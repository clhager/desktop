using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexColorCoordinates
{
    public static Color GetColorCoordinates(Vector2Int coordinates)
    {
        return new Color(coordinates.x, coordinates.y, 0, 0);
    }
}
