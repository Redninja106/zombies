namespace ConsoleApp36.Particles;

internal class ParticleStateContainer<TBehavior> : IParticleBehaviorContainer
    where TBehavior : IParticleBehavior
{
    private readonly ParticleSystem system;
    private readonly TBehavior[] behaviors;
    private readonly Func<ParticleSystem, TBehavior>? provider;
    public IParticleBehaviorState? SharedState { get; set; } = null;

    public ParticleStateContainer(ParticleSystem system, int size, Func<ParticleSystem, TBehavior>? provider)
    {
        this.system = system;
        this.behaviors = new TBehavior[size];
        this.provider = provider;
    }

    public void Initialize(int index, Random random)
    {
        if (provider is not null)
        {
            behaviors[index] = provider(this.system);
        }
        else
        {
            behaviors[index] = Activator.CreateInstance<TBehavior>();
        }
    }

    public void Update(int activeParticleCount)
    {
        for (int i = 0; i < activeParticleCount; i++)
        {
            behaviors[i].Update(new(this.system, i));
        }

        SharedState?.Update();
    }

    public void Render(ICanvas canvas, int activeParticleCount)
    {
        for (int i = 0; i < activeParticleCount; i++)
        {
            behaviors[i].Render(canvas, new(this.system, i));
        }

        SharedState?.Render(canvas);
    }

    public void Swap(int indexA, int indexB)
    {
        (behaviors[indexA], behaviors[indexB]) = (behaviors[indexB], behaviors[indexA]);
    }

    public ref TBehavior this[int index] => ref behaviors[index];
}
