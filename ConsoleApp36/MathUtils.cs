using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal static class MathUtils
{
    public static float TimescaledLerpFactor(float smoothing, float dt)
    {
        return 1f - MathF.Exp(-smoothing * dt);
    }
}
