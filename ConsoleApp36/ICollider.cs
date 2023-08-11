using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal interface ICollider : IGameComponent
{
    ColliderKind Kind { get; }
    float Mass { get; }

    ReadOnlySpan<Vector2> GetPolygon();
    void OnCollision(ICollider other, Vector2 mtv);
    bool FilterCollision(ICollider collider) => true;
}