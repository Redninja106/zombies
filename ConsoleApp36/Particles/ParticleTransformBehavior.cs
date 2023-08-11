namespace ConsoleApp36.Particles;

struct ParticleTransformBehavior : IParticleBehavior
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Right = Vector2.UnitX;
    public Vector2 Scale = Vector2.One;
    public readonly Vector2 Up => new(-Right.Y, Right.X);
    
    public ParticleTransformBehavior(Transform transform) : this(transform.WorldPosition, transform.WorldRotation)
    {
    }

    public ParticleTransformBehavior(Transform transform, Vector2 localOffset, float localOffsetAngle = 0) : this(transform.LocalToWorld(transform.Position + localOffset), transform.WorldRotation + localOffsetAngle)
    {
    }


    public ParticleTransformBehavior() : this(Vector2.Zero, 0)
    {
    }

    public ParticleTransformBehavior(Vector2 position, float rotation)
    {
        this.Position = position;
        this.SetRotation(rotation);
    }

    public readonly Matrix3x2 CreateLocalToWorldMatrix()
    {
        return new Matrix3x2(
            Right.X * Scale.X, Right.Y * Scale.X,
            Up.X * Scale.Y, Up.Y * Scale.Y,
            Position.X, Position.Y
            );
    }

    public readonly float GetRotation()
    {
        return Angle.FromVector(Right);
    }

    public void SetRotation(float rotation)
    {
        Right = Angle.ToVector(rotation);
    }

    public void Initialize(Random random)
    {
    }

    public void Update(Particle particle)
    {
    }

    public void Render(ICanvas canvas, Particle particle)
    {
    }
}
