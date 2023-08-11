using Silk.NET.SDL;
using SimulationFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;

public class Transform
{
    private Vector2 position;
    private float rotation;
    private Vector2 scale;
    private Transform? parent;

    // (parent) -> position -> rotation -> scale -> (children)

    public ref Vector2 Position => ref position;
    public ref float Rotation => ref rotation;
    public ref Vector2 Scale => ref scale;
    public Transform? Parent => parent;

    public Vector2 WorldPosition
    {
        get
        {
            if (this.parent is null)
                return this.position;

            return this.parent.LocalToWorld(this.position);
        }
        set
        {
            if (this.parent is null)
            {
                this.position = value;
                return;
            }

            this.position = this.parent.WorldToLocal(value);
        }
    }

    public float WorldRotation
    {
        get
        {
            if (this.parent is null) 
            {
                return this.rotation;
            }
            else
            {
                return this.parent.WorldRotation + this.rotation;
            }
        }
        set
        {
            if (this.parent is null)
            {
                this.rotation = value;
            }
            else
            {
                this.rotation = value - this.parent.WorldRotation;
            }
        }
    }

    public Vector2 WorldScale
    { 
        get
        {
            if (this.parent is null)
            {
                return this.scale;
            }
            else
            {
                return this.parent.WorldScale * this.scale;
            }
        }
        set
        {
            if (this.parent is null)
            {
                this.scale = value;
            }
            else
            {
                this.scale = value / this.parent.WorldScale;
            }
        }
    }

    public Vector2 Right
    {
        get => Vector2.UnitX.Rotated(Rotation);
    }

    public Vector2 Left
    {
        get => (-Vector2.UnitX).Rotated(Rotation);
    }

    public Vector2 Up
    {
        get => (-Vector2.UnitY).Rotated(Rotation);
    }

    public Vector2 Down
    {
        get => Vector2.UnitY.Rotated(Rotation);
    }

    public Transform(Transform? parent = null) : this(0, 0, 0, parent)
    {
    }

    public Transform(float x, float y, float r, Transform? parent = null) : this(new(x, y), r, parent)
    {
    }

    public Transform(Vector2 position, float rotation, Transform? parent = null) : this(position, rotation, Vector2.One, parent)
    {
    }

    public Transform(Vector2 position, float rotation, Vector2 scale, Transform? parent = null)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.parent = parent;
    }
    public void Translate(float x, float y, bool local = true)
    {
        Translate(new(x, y), local);
    }

    public void Translate(Vector2 translation, bool local = true)
    {
        if (local)
        {
            Position += translation;
        }
        else
        {
            WorldPosition += translation;
        }
    }

    public void Rotate(float rotation)
    {
        Rotation += rotation;
    }

    public void ScaleBy(Vector2 scale)
    {
        this.scale *= scale;
    }

    public Vector2 WorldToLocal(Vector2 point)
    {
        return Vector2.Transform(point, CreateWorldToLocalMatrix());
    }

    public Vector2 LocalToWorld(Vector2 point)
    {
        return Vector2.Transform(point, CreateLocalToWorldMatrix());
    }

    public Vector2 ParentToLocal(Vector2 point)
    {
        return Vector2.Transform(point, CreateParentToLocalMatrix());
    }

    public Vector2 LocalToParent(Vector2 point)
    {
        return Vector2.Transform(point, CreateLocalToParentMatrix());
    }

    public Matrix3x2 CreateWorldToLocalMatrix()
    {
        if (parent is null)
            return CreateParentToLocalMatrix();

        return parent.CreateWorldToLocalMatrix() * this.CreateParentToLocalMatrix();
    }

    public Matrix3x2 CreateLocalToWorldMatrix()
    {
        if (parent is null)
            return CreateLocalToParentMatrix();

        return this.CreateLocalToParentMatrix() * parent.CreateLocalToWorldMatrix();
    }

    public Matrix3x2 CreateParentToLocalMatrix()
    {
        var matrix = Matrix3x2.CreateScale(Vector2.One / this.Scale);
        matrix = matrix.Append(Matrix3x2.CreateRotation(-this.Rotation));
        matrix = matrix.Append(Matrix3x2.CreateTranslation(-this.Position));
        return matrix;
    }

    public Matrix3x2 CreateLocalToParentMatrix()
    {
        var matrix = Matrix3x2.CreateTranslation(this.Position);
        matrix = matrix.Append(Matrix3x2.CreateRotation(this.Rotation));
        matrix = matrix.Append(Matrix3x2.CreateScale(this.Scale));
        return matrix;
    }

    public override string ToString()
    {
        return $"({Position.X},{Position.Y},{Rotation})";
    }

    public static Transform Parse(string str)
    {
        var parts = str.Trim('(', ')').Split(',');
        float x = float.Parse(parts[0]);
        float y = float.Parse(parts[1]);
        float r = float.Parse(parts[2]);
        return new(x, y, r);
    }

    public void Match(Transform other)
    {
        this.Position = other.Position;
        this.Rotation = other.Rotation;
        this.Scale = other.Scale;
    }

    public void LerpTowards(Transform other, float t)
    {
        this.Position = Vector2.Lerp(this.Position, other.Position, t);
        this.Rotation = MathHelper.Lerp(this.Rotation, other.Rotation, t);
        this.Scale = Vector2.Lerp(this.Scale, other.Scale, t);
    }

    public void Reparent(Transform? newParent)
    {
        this.parent = newParent;
    }

    public void LookAt(Vector2 point, bool local = true)
    {
        if (local)
        {
            this.rotation = Angle.FromVector(point - this.position);
        }
        else
        {
            this.WorldRotation = Angle.FromVector(point - this.WorldPosition);
        }
    }

    public void StepTowards(Vector2 point, float distance)
    {
        Vector2 diff = point - position;
        float lenSq = point.LengthSquared();

        if (lenSq < distance * distance)
        {
            position = point;
            return;
        }

        diff *= MathF.ReciprocalSqrtEstimate(lenSq);
        position += diff * distance;
    }

    public void TurnTowards(Vector2 point, float radians, bool local = true)
    {
        if (local)
        {
            this.rotation = Angle.Step(this.rotation, Angle.FromVector(point - this.position), radians);
        }
        else
        {
            this.WorldRotation = Angle.Step(this.WorldRotation, Angle.FromVector(point - this.WorldPosition), radians);
        }
    }
}