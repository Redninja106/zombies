using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal static class PolygonUtils
{
    public static Vector2[] CreateCircle(int segments, float radius)
    {
        return CreateEllipse(segments, radius, radius);
    }

    public static Vector2[] CreateRect(Rectangle rectangle)
    {
        var result = new Vector2[4];
        result[0] = rectangle.GetAlignedPoint(Alignment.TopLeft);
        result[1] = rectangle.GetAlignedPoint(Alignment.TopRight);
        result[2] = rectangle.GetAlignedPoint(Alignment.BottomRight);
        result[3] = rectangle.GetAlignedPoint(Alignment.BottomLeft);
        return result;
    }

    public static Vector2[] CreateEllipse(int segments, float radiusX, float radiusY)
    {
        var result = new Vector2[segments];
        for (int i = 0; i < segments; i++)
        {
            result[i] = new Vector2(radiusX, radiusY) * Angle.ToVector(MathF.Tau * (i / (float)segments));
        }
        return result;
    }
}
