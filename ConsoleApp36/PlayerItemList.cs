using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;

internal class PlayerItemList : GameComponent
{
    private readonly Dictionary<Item, float> items = new();

    public IEnumerable<Item> Items => items.Keys;

    public override RenderLayer RenderLayer => RenderLayer.GUI;

    public void AddItem(Item item, float count = 1)
    {
        if (items.TryGetValue(item, out float oldCount))
        {
            items[item] = oldCount + count;
        }
        else
        {
            items.Add(item, count);
        }
    }

    public float GetItemCount(Item item)
    {
        return items[item];
    }

    public PlayerAbilities GetPlayerAbilities()
    {
        PlayerAbilities abilities = new();

        foreach (var (item, count) in items)
        {
            item.ApplyTo(abilities, count);
        }

        return abilities;
    }

    public override void Update()
    {
    }

    public override void Render(ICanvas canvas)
    {
        if (items.Count is 0)
            return;

        canvas.ResetState();
        canvas.Transform(Program.Camera.LocalToScreen.Matrix);
        canvas.Scale(Program.Camera.VerticalSize);
        Rectangle viewport = new(0, 0, Program.Camera.HorizontalSize / Program.Camera.VerticalSize, 1, Alignment.Center);

        float panelHeight = .01f + .06f * MathF.Ceiling(items.Count / 10f);

        Rectangle panel = new(viewport.GetAlignedPoint(Alignment.BottomCenter), new(.61f, panelHeight), Alignment.BottomCenter);

        canvas.Fill(new Color(0, 0, 0, 128));
        canvas.DrawRect(panel);

        canvas.Translate(panel.GetAlignedPoint(Alignment.TopLeft));

        int itemsInRow = 0;
        canvas.PushState();
        foreach (var (item, count) in items)
        {
            canvas.PushState();
            Rectangle itemBounds = new(.01f, .01f, .05f, .05f);
            item.Render(canvas, count, itemBounds);
            canvas.PopState();
            canvas.FontSize(.025f);
            canvas.Fill(Color.White);
            canvas.DrawText(count.ToString(), itemBounds.GetAlignedPoint(Alignment.BottomRight) - Vector2.One * .005f, Alignment.BottomRight);

            canvas.Translate(.06f, 0);
            itemsInRow++;
            if (itemsInRow % 10 is 0)
            {
                canvas.PopState();
                canvas.Translate(0, .06f);
                canvas.PushState();
            }
        }

        canvas.PopState();
    }
}
