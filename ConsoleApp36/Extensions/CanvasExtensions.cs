using ConsoleApp36.Particles;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36.Extensions;
internal static class CanvasExtensions
{
    public static void ApplyTransform(this ICanvas canvas, Transform transform)
    {
        canvas.Transform(transform.CreateLocalToWorldMatrix());
    }

    public static void ApplyTransform(this ICanvas canvas, ParticleTransformBehavior transform)
    {
        canvas.Transform(transform.CreateLocalToWorldMatrix());
    }
}