using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36.Extensions;
internal static class Matrix3x2Extensions
{
    public static Matrix3x2 Append(this Matrix3x2 self, Matrix3x2 matrix)
    {
        return matrix * self;
    }
}
