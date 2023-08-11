using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;

internal class CollisionManager : GameComponent
{
    public bool DrawColliders { get; set; } = false;
    private ICollider[] colliders;
    private ContactManager contactManager = new();

    public CollisionManager()
    {
    }

    public override void Update()
    {
        colliders = Program.World.FindAll<ICollider>().ToArray();
        contactManager.ClearContacts();

        for (int i = 0; i < colliders.Length; i++)
        {
            for (int j = i+1; j < colliders.Length; j++)
            {
                var collider = colliders[i];
                var other = colliders[j];

                if (collider.Kind is ColliderKind.Static && other.Kind is ColliderKind.Static)
                    continue;
                if (collider.Kind is ColliderKind.Ghost && other.Kind is ColliderKind.Ghost)
                    continue;
                if (!collider.FilterCollision(other) || !other.FilterCollision(collider))
                    continue;

                var colliderPoly = collider.GetPolygon();
                var colliderTransform = collider.Transform;

                var otherPoly = other.GetPolygon();
                var otherTransform = other.Transform;

                if (SATCollision(colliderPoly, colliderTransform, otherPoly, otherTransform, out Vector2 mtv))
                {
                    float massRatio = collider.Mass / (collider.Mass + other.Mass);
                    var wa = 1 - massRatio;
                    var wb = massRatio;

                    if (collider.Kind is ColliderKind.Dynamic && other.Kind is ColliderKind.Dynamic or ColliderKind.Kinematic or ColliderKind.Static)
                    { 
                        collider.Transform.WorldPosition += mtv * wa;
                    }
                    if (other.Kind is ColliderKind.Dynamic && collider.Kind is ColliderKind.Dynamic or ColliderKind.Kinematic or ColliderKind.Static)
                    {
                        other.Transform.WorldPosition += -mtv * wb;
                    }

                    contactManager.AddContact(collider, other, mtv);
                    
                    if (other.Kind is not ColliderKind.Ghost)
                        collider.OnCollision(other, mtv);
                    
                    if (collider.Kind is not ColliderKind.Ghost)
                        other.OnCollision(collider, -mtv);
                }
            }
        }
    }

    public IEnumerable<Contact> GetContacts(ICollider collider)
    {
        return contactManager.GetContacts(collider);
    }

    public override void Render(ICanvas canvas)
    {
        if (DrawColliders)
        {
            var colliders = Program.World.FindAll<ICollider>();

            foreach (var collider in colliders)
            {
                DrawShape(canvas, collider.Transform, collider.GetPolygon());
            }
        }
    }

    void DrawShape(ICanvas canvas, Transform transform, ReadOnlySpan<Vector2> shape)
    {
        canvas.PushState();
        canvas.ApplyTransform(transform);

        canvas.Stroke(Color.Red);
        canvas.DrawPolygon(shape, true);
        canvas.PopState();
    }

    private bool SATCollision(ReadOnlySpan<Vector2> shapeA, Transform transformA, ReadOnlySpan<Vector2> shapeB, Transform transformB, out Vector2 mtv)
    {
        Span<Vector2> axes = stackalloc Vector2[shapeA.Length + shapeB.Length];
        GetAxes(shapeA, axes, transformA);
        GetAxes(shapeB, axes[(shapeA.Length)..], transformB);

        float leastOverlap = float.PositiveInfinity;
        Vector2 leastOverlapAxis = default;

        for (int i = 0; i < axes.Length; i++)
        {
            Project(shapeA, transformA, axes[i], out float aMin, out float aMax);
            Project(shapeB, transformB, axes[i], out float bMin, out float bMax);

            var x = bMin - aMax;
            var y = aMin - bMax;

            if (MathF.Sign(x) + MathF.Sign(y) != 0)
            {
                var overlap = MathF.MinMagnitude(x, -y);
                if (MathF.Abs(overlap) < MathF.Abs(leastOverlap))
                {
                    leastOverlap = overlap;
                    leastOverlapAxis = axes[i];
                }

                continue;
            }

            mtv = default;
            return false;
        }

        mtv = leastOverlapAxis * leastOverlap;
        return true;

        static void GetAxes(ReadOnlySpan<Vector2> shape, Span<Vector2> axes, Transform transform)
        {
            for (int i = 0; i < shape.Length; i++)
            {
                Vector2 side = transform.LocalToWorld(shape[i]) - transform.LocalToWorld(shape[i + 1 >= shape.Length ? 0 : i + 1]);
                axes[i] = new(side.Y, -side.X);
                axes[i] = axes[i].Normalized();
            }
        }

        static void Project(ReadOnlySpan<Vector2> shape, Transform transform, Vector2 axis, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;

            for (int i = 0; i < shape.Length; i++)
            {
                Vector2 vertex = transform.LocalToWorld(shape[i]);

                float dot = Vector2.Dot(vertex, axis);

                min = Math.Min(min, dot);
                max = Math.Max(max, dot);
            }
        }
    }

    public bool RayCast(Vector2 position, Vector2 direction, float maxDistance, out RayCastHit hit)
    {
        return RayCast(position, direction, maxDistance, null, out hit);
    }

    public bool RayCast(Vector2 position, Vector2 direction, float maxDistance, Predicate<ICollider>? filter, out RayCastHit hit)
    {
        float closest = maxDistance;
        ICollider? closestCollider = null;
        Vector2 closestNormal = default;

        foreach (var collider in colliders)
        {
            if (filter is not null && !filter(collider))
                continue;

            var obb = collider.GetPolygon();

            for (int i = 0; i < obb.Length; i++)
            {
                Vector2 from = collider.Transform.LocalToWorld(obb[i]);
                Vector2 to = collider.Transform.LocalToWorld(obb[i + 1 >= obb.Length ? 0 : (i + 1)]);

                float? dist = RayLineIntersect(position, direction, from, to);
                if (dist is not null)
                {
                    if (dist < closest)
                    {
                        closest = dist.Value;
                        closestCollider = collider;
                        Vector2 dir = (to - from);
                        closestNormal = new Vector2(dir.Y, -dir.X).Normalized();
                    }
                }
            }
        }

        hit.position = position;
        hit.direction = direction;
        hit.distance = closest;
        hit.collider = closestCollider;
        hit.normal = closestNormal;

        return hit.collider is not null;
    }

    public float? RayLineIntersect(Vector2 position, Vector2 direction, Vector2 from, Vector2 to)
    {
        var v1 = position - from;
        var v2 = to - from;
        var v3 = new Vector2(-direction.Y, direction.X);


        var dot = Vector2.Dot(v2, v3);
        if (Math.Abs(dot) < 0.0001f)
            return null;

        var t1 = Cross(v2, v1) / dot;
        var t2 = Vector2.Dot(v1, v3) / dot;

        if (t1 >= 0.0f && (t2 >= 0.0f && t2 <= 1.0f))
            return t1;

        return null;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return Vector3.Cross(new(a, 0), new(b, 0)).Z;
    }

    public bool TestPoint(Vector2 point, Predicate<ICollider>? filter = null)
    {
        return TestPoint(point, filter, out _);
    }

    public bool TestPoint(Vector2 point, Predicate<ICollider>? filter, [NotNullWhen(true)] out ICollider? hit)
    {
        foreach (var collider in colliders)
        {
            if (filter is not null && !filter(collider))
                continue;

            var localPoint = Transform.WorldToLocal(point);

            var poly = collider.GetPolygon();
            var bounds = Polygon.GetBoundingBox(poly);

            if (!bounds.ContainsPoint(localPoint))
                continue;

            int intersections = 0;
            for (int i = 0; i < poly.Length; i++)
            {
                Vector2 from = collider.Transform.LocalToWorld(poly[i]);
                Vector2 to = collider.Transform.LocalToWorld(poly[i + 1 >= poly.Length ? 0 : (i + 1)]);

                if (RayLineIntersect(localPoint, Vector2.UnitX, from, to) is not null)
                {
                    intersections++;
                }
            }

            if (intersections % 2 is 1)
            {
                hit = collider;
                return true;
            }
        }

        hit = null;
        return false;
    }
}

struct RayCastHit
{
    public Vector2 position;
    public Vector2 direction;
    public float distance;
    public ICollider? collider;
    public Vector2 normal;

    public readonly Vector2 Point => position + direction * distance;
}

class ContactManager
{
    private readonly List<Contact> contacts = new();

    internal void ClearContacts()
    {
        contacts.Clear();
    }

    internal void AddContact(ICollider colliderA, ICollider colliderB, Vector2 mtv)
    {
        contacts.Add(new(colliderA, colliderB, mtv));
    }

    public IEnumerable<Contact> GetContacts(ICollider collider)
    {
        return contacts.Where(c => c.ColliderA == collider || c.ColliderB == collider);
    }
}

record Contact(ICollider ColliderA, ICollider ColliderB, Vector2 MinimumTranslationValue)
{
}