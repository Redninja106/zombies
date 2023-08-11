namespace ConsoleApp36.Particles;

interface IParticleBehavior
{
    void Update(Particle particle);
    void Render(ICanvas canvas, Particle particle);
}
