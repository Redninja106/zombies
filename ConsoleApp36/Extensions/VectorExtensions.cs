using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36.Extensions;
internal static class VectorExtensions
{
    public static Vector2 Rotated(this Vector2 vector, float rotation)
    {
        var (s, c) = MathF.SinCos(rotation);
        return new(vector.X * c - vector.Y * s, vector.X * s + vector.Y * c);
    }

    public static Vector2 StepTowards(this Vector2 vector, Vector2 target, float amount)
    {
        float distance = Vector2.Distance(vector, target);

        if (distance < amount)
            return target;

        return vector + (target - vector).Normalized() * amount;
    }
}
