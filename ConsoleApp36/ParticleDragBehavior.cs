using ConsoleApp36.Particles;

namespace ConsoleApp36;

internal struct ParticleDragBehavior : IParticleBehavior
{
    public void Render(ICanvas canvas, Particle particle)
    {
    }

    [DependencyOf<ParticleVelocityBehavior>]
    public void Update(Particle particle)
    {
        ref var velocity = ref particle.GetBehavior<ParticleVelocityBehavior>();
        velocity.velocity *= MathF.Pow(.9f, Time.DeltaTime);
        velocity.angularVelocity *= MathF.Pow(.9f, Time.DeltaTime);
    }
}