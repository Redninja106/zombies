using ConsoleApp36.Particles;

namespace ConsoleApp36;

internal struct ParticleLifetimeBehavior : IParticleBehavior
{
    public float Lifetime { get; set; }
    private float age;

    public ParticleLifetimeBehavior(float lifetime)
    {
        this.Lifetime = lifetime;
        this.age = 0;
    }

    public void Render(ICanvas canvas, Particle particle)
    {
    }

    public void Update(Particle particle)
    {
        age += Time.DeltaTime;

        if (age >= Lifetime)
        {
            particle.Destroy();
        }
    }
}