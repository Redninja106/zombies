namespace ConsoleApp36.Particles;

internal interface IParticleBehaviorState
{
    void Update();
    void Render(ICanvas canvas);
}