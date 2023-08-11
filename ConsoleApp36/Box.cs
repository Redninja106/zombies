using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal class Box : GameComponent, ICollider
{
    private Vector2[] collider = new Vector2[4];

    public float Width { get; }
    public float Height { get; }
    public ColliderKind Kind => ColliderKind.Dynamic;
    public override RenderLayer RenderLayer => RenderLayer.Obsctacles;
    public float Mass => Width * Height * 100;

    public Box(float x, float y, float width, float height, Transform? parent = null) : base(parent)
    {
        this.Transform.Position = new(x,y);
        this.Width = width;
        this.Height = height;
    }

    public override void Update()
    {
        var rect = new Rectangle(0, 0, Width, Height, Alignment.Center);
        collider[0] = rect.GetAlignedPoint(Alignment.TopLeft);
        collider[1] = rect.GetAlignedPoint(Alignment.TopRight);
        collider[2] = rect.GetAlignedPoint(Alignment.BottomRight);
        collider[3] = rect.GetAlignedPoint(Alignment.BottomLeft);
    }

    public override void Render(ICanvas canvas)
    {
        base.Render(canvas);
        canvas.DrawRect(0, 0, Width, Height, Alignment.Center);
    }

    public ReadOnlySpan<Vector2> GetPolygon() => collider;

    public void OnCollision(ICollider other, Vector2 mtv)
    {
    }
}