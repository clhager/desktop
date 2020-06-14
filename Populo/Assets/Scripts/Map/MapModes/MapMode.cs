using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MapMode : Model
{
    public MapMode() : base() { }

    public MapMode(ReadMap.ObjectMap map) : base(map) { }
    
    public abstract Color[,] GetMapColors();

    public abstract class ColorMapMode : MapMode 
    {
        public ColorMapMode() : base() { }
        public ColorMapMode(ReadMap.ObjectMap map) : base(map) { }

        public abstract Color GetHexColor(HexModel hexModel);

        public override Color[,] GetMapColors()
        {
            Color[,] colors = new Color[MapModel.GetWidth(), MapModel.GetHeight()];
            for (int i = 0; i < colors.GetLength(0); i++)
            {
                for (int j = 0; j < colors.GetLength(0); j++)
                {
                    colors[i, j] = GetHexColor(HexModel.map[new Vector2Int(i, j)]);
                }
            }
            return colors;
        }
    }

    public abstract class ValueMapMode : MapMode
    {
        public Color MinColor { get; set; }
        public Color MaxColor { get; set; }

        public ValueMapMode(ReadMap.ObjectMap map) : base(map) { }

        public ValueMapMode(Color minColor, Color maxColor) : base()
        {
            MinColor = minColor;
            MaxColor = maxColor;
        }

        public abstract float GetHexValue(HexModel hexModel);

        public override Color[,] GetMapColors()
        {
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            float[,] values = new float[MapModel.GetWidth(), MapModel.GetHeight()];
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(0); j++)
                {
                    values[i, j] = GetHexValue(HexModel.map[new Vector2Int(i, j)]);
                    minValue = Mathf.Min(minValue, values[i, j]);
                    maxValue = Mathf.Max(maxValue, values[i, j]);
                }
            }
            Color[,] colors = new Color[MapModel.GetWidth(), MapModel.GetHeight()];
            for (int i = 0; i < colors.GetLength(0); i++)
            {
                for (int j = 0; j < colors.GetLength(0); j++)
                {
                    colors[i, j] = Color.Lerp(MinColor, MaxColor, (values[i, j] - minValue) / (maxValue - minValue));
                }
            }
            return colors;
        }
    }

    public class TerrainMapMode : ColorMapMode
    {
        public TerrainMapMode() : base() { }

        public TerrainMapMode(ReadMap.ObjectMap map) : base(map) { }

        public override Color GetHexColor(HexModel hexModel)
        {
            return TerrainModel.GetColor(hexModel);
        }
    }

    public class ElevationMapMode : ValueMapMode
    {
        public ElevationMapMode() : base(new Color(.8f, .75f, .4f), new Color(.65f, .1f, 0f)) { } 

        public override float GetHexValue(HexModel hexModel)
        {
            return hexModel.Elevation;
        }
    }

}
