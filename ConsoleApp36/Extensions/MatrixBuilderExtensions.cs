using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36.Extensions;
internal static class MatrixBuilderExtensions
{
    public static MatrixBuilder ApplyTransform(this MatrixBuilder matrixBuilder, Transform transform)
    {
        return matrixBuilder
            .Translate(transform.Position)
            .Rotate(transform.Rotation);
    }
}
