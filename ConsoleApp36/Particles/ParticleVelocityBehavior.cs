namespace ConsoleApp36.Particles;

struct ParticleVelocityBehavior : IParticleBehavior
{
    public Vector2 velocity;
    public float angularVelocity;

    public ParticleVelocityBehavior(Vector2 velocity, float angularVelocity)
    {
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;
    }

    public void Initialize(Random random)
    {
        velocity = random.NextUnitVector2();
    }

    public void Render(ICanvas canvas, Particle particle)
    {
    }

    [DependencyOf<ParticleTransformBehavior>]
    public void Update(Particle particle)
    {
        ref var transform = ref particle.GetBehavior<ParticleTransformBehavior>();
        transform.Position += velocity * Time.DeltaTime;
    }
}