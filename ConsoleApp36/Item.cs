using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal abstract class Item
{
    public virtual ITexture? Texture => null;

    public abstract void ApplyTo(PlayerAbilities abilities, float count);

    public virtual void Update(float count)
    {
    }

    public virtual void Render(ICanvas canvas, float count, Rectangle destination)
    {
        if (Texture is not null)
        {
            canvas.DrawTexture(Texture, destination);
        }
    }
}

class PlayerAbilities
{
    public float MoveSpeed { get; set; } = 2.5f;
    public float Acceleration { get; set; } = 25f;
    public float Damage { get; set; } = 1;
}

class SpeedBoostItem : Item
{
    public static readonly SpeedBoostItem Instance = new();

    public override void ApplyTo(PlayerAbilities abilities, float count)
    {
        abilities.MoveSpeed *= 1 + .1f * count;
        abilities.Acceleration *= 1 + .1f * count;
    }

    public override void Render(ICanvas canvas, float count, Rectangle destination)
    {
        canvas.Fill(Color.Blue);
        canvas.DrawRect(destination);
    }
}

class DamageItem : Item
{
    public static readonly DamageItem Instance = new();

    public override void ApplyTo(PlayerAbilities abilities, float count)
    {
        abilities.Damage *= 1 + .1f * count;
    }

    public override void Render(ICanvas canvas, float count, Rectangle destination)
    {
        canvas.Fill(Color.Red);
        canvas.DrawRect(destination);
    }
}