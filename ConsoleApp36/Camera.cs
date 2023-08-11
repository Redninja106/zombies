using SimulationFramework;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;

internal class Camera : GameComponent
{
    public readonly MatrixBuilder WorldToLocal = new();
    public readonly MatrixBuilder LocalToScreen = new();

    public float AspectRatio { get; private set; }

    public float VerticalSize { get; set; }
    public float HorizontalSize { get => VerticalSize * AspectRatio; set => VerticalSize = value / AspectRatio; }

    public int DisplayWidth { get; private set; }
    public int DisplayHeight { get; private set; }

    public Camera(float verticalSize)
    {
        VerticalSize = verticalSize;
    }

    public override void Update()
    {
        this.Transform.Rotation = Angle.Normalize(this.Transform.Rotation);

        this.DisplayWidth = Window.Width;
        this.DisplayHeight = Window.Height;

        AspectRatio = DisplayWidth / (float)DisplayHeight;

        LocalToScreen
            .Reset()
            .Translate(DisplayWidth / 2f, DisplayHeight / 2f)
            .Scale(DisplayHeight / VerticalSize);

        WorldToLocal
            .Reset()
            .Rotate(-Transform.Rotation)
            .Translate(-Transform.Position);
    }

    public void ApplyTo(ICanvas canvas)
    {
        canvas.Transform(LocalToScreen.Matrix);
        canvas.Transform(WorldToLocal.Matrix);
    }

    public Vector2 ScreenToWorld(Vector2 point)
    {
        point = Vector2.Transform(point, LocalToScreen.InverseMatrix);
        point = Vector2.Transform(point, WorldToLocal.InverseMatrix);
        return point;
    }
}