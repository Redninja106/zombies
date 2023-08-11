using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;

enum ColliderKind
{
    // normal collision behavior; objects can be moved to resolve collisions
    Dynamic,
    // normal collision behavior; kinematic objects are never moved to resolve collisions.
    Kinematic,
    // like kinematic, but not checked against other static objects. only for objects that don't move.
    Static,
    // full collision notifications with no attempts to resolve them.
    Projectile,
    // ghost colliders are notified when they intersect another non-ghost collider, but the other collider is not notified and no attempt is made to resolve the collision.
    Ghost,
}