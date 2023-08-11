using ConsoleApp36;
using SimulationFramework;
using SimulationFramework.Drawing;

Start<Program>();

partial class Program : Simulation
{
    public static GameWorld World { get; } = new();
    public static Camera Camera => World.Find<Camera>();
    public static CollisionManager Collision => World.Find<CollisionManager>();

    public override void OnInitialize()
    {
        World.Register(new Camera(20));
        World.Register(new Player());
        // World.Register(new Box(2, 4));
        var rotor = new Rotor(2, -4);
        World.Register(rotor);
        World.Register(new Box(0, 0, 10, 1, rotor.Transform));
        World.Register(new Box(0,0, 1, 1));
        World.Register(new Zombie(5, -5));

        World.Register(new CollisionManager());
    }

    public override void OnRender(ICanvas canvas)
    {
        Camera.Update();
        World.Update();
        Camera.Update();

        canvas.Clear(Color.FromHSV(0, 0, .1f));
        canvas.Antialias(true);
        Camera.ApplyTo(canvas);
        World.Render(canvas);
    }

}