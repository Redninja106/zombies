using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;

internal class ItemPickup : GameComponent, ICollider
{
    private Item item;
    private float count;

    public override RenderLayer RenderLayer => RenderLayer.Pickups;

    public ColliderKind Kind => ColliderKind.Dynamic;
    public float Mass => 0f;
    private static Vector2[] collider = PolygonUtils.CreateRect(new(0, 0, .5f, .5f, Alignment.Center));

    bool ICollider.FilterCollision(ICollider collider)
    {
        return collider.RenderLayer is RenderLayer.Background or RenderLayer.Obsctacles;
    }

    public ItemPickup(Vector2 position, Item item, float count)
    {
        this.Transform.Position = position;
        this.item = item;
        this.count = count;
    }

    public override void Update()
    {
        var player = Program.World.Find<Player>();
        var dir = player.Transform.WorldPosition - this.Transform.WorldPosition;
        var dist = dir.Length();

        if (dist < 3f)
        {
            Transform.StepTowards(player.Transform.Position, 250f / dist * Time.DeltaTime);
        }

        if (dist < 0.5f)
        {
            player.ItemList.AddItem(this.item, count);
            Program.World.Destroy(this);
        }
    }

    public override void Render(ICanvas canvas)
    {
        base.Render(canvas);
        item.Render(canvas, this.count, new(0, 0, .5f, .5f, Alignment.Center));
    }

    public ReadOnlySpan<Vector2> GetPolygon()
    {
        return collider;
    }

    public void OnCollision(ICollider other, Vector2 mtv)
    {
    }
}
