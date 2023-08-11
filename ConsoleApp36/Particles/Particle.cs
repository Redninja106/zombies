namespace ConsoleApp36.Particles;

struct Particle
{
    public ParticleSystem System { get; set; }
    public int Index { get; set; }

    public Particle(ParticleSystem system, int index)
    {
        System = system;
        Index = index;
    }

    public readonly ref TState GetBehavior<TState>()
        where TState : IParticleBehavior
    {
        var container = System.GetBehaviorContainer<TState>();
        return ref container[Index];
    }

    public readonly void Destroy()
    {
        System.Remove(Index);
    }
}
