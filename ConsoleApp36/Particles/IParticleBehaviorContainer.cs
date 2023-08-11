namespace ConsoleApp36.Particles;

internal interface IParticleBehaviorContainer
{
    void Initialize(int index, Random random);
    void Update(int activeParticleCount);
    void Render(ICanvas canvas, int activeParticleCount);
    void Swap(int indexA, int indexB);
}
