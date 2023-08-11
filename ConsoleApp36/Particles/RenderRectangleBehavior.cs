using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36.Particles;
internal struct RenderRectangleBehavior : IParticleBehavior
{
    public float Size { get; set; }
    public Color Color { get; set; }

    public RenderRectangleBehavior() : this(.1f, Color.White)
    {
    }

    public RenderRectangleBehavior(float size, Color color)
    {
        Size = size;
        Color = color;
    }

    public void Render(ICanvas canvas, Particle particle)
    {
        var transform = particle.GetBehavior<ParticleTransformBehavior>();
        canvas.PushState();
        canvas.Transform(transform.CreateLocalToWorldMatrix());
        canvas.Fill(Color);
        canvas.DrawRect(0, 0, Size, Size, Alignment.Center);
        canvas.PopState();
    }

    public void Update(Particle particle)
    {
    }
}
