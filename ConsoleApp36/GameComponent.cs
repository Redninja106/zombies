using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;

interface IGameComponent
{
    RenderLayer RenderLayer { get; }
    Transform Transform { get; }
    void Update();
    void Render(ICanvas canvas);
}

abstract class GameComponent : IGameComponent
{
    public virtual RenderLayer RenderLayer => RenderLayer.Unspecified;
    public Transform Transform { get; init; }
    protected GameWorld World { get; }

    public GameComponent(Transform? parent = null)
    {
        World = Program.World;
        this.Transform = new(parent);
    }

    public abstract void Update();

    public virtual void Render(ICanvas canvas)
    {
        canvas.ApplyTransform(Transform);
    }
}