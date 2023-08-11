using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal class Bullet : GameComponent, ICollider
{
    private float damage;
    private bool destroyed;
    private Vector2 velocity;
    
    public float Mass => 1;

    public Bullet(float damage, Vector2 pos, float rotation, Vector2 shooterVelocity)
    {
        this.Transform.Position = pos;
        this.Transform.Rotation = rotation;

        velocity = this.Transform.Right * 50 + shooterVelocity;
        this.damage = damage;
    }


    private static Vector2[] polygon;
    public ColliderKind Kind => ColliderKind.Projectile;

    static Bullet()
    {
        Rectangle rect = new(0, 0, 1f, .05f, Alignment.Center);
        polygon = new Vector2[4];
        GetAABB(rect, polygon);
    }

    public ReadOnlySpan<Vector2> GetPolygon()
    {
        return polygon;
    }

    public override void Render(ICanvas canvas)
    {
        base.Render(canvas);
        canvas.Fill(Color.Yellow);
        canvas.DrawRect(0, 0, 1f, .05f, Alignment.Center);
    }

    public override void Update()
    {
        this.Transform.Translate(velocity * Time.DeltaTime);

        if (this.Transform.WorldPosition.LengthSquared() > 1000 * 1000)
        {
            Program.World.Destroy(this);
        }
    }
    private static void GetAABB(Rectangle rectangle, Span<Vector2> result)
    {
        result[0] = rectangle.GetAlignedPoint(Alignment.TopLeft);
        result[1] = rectangle.GetAlignedPoint(Alignment.TopRight);
        result[2] = rectangle.GetAlignedPoint(Alignment.BottomRight);
        result[3] = rectangle.GetAlignedPoint(Alignment.BottomLeft);
    }

    public void OnCollision(ICollider other, Vector2 mtv)
    {
        if (destroyed || other is Bullet)
        {
            return;
        }

        if (other is IDamagable damagable)
        {
            damagable.Damage(damage, DamageKind.Bullet);
            destroyed = true;
        }

        Program.World.Destroy(this);
    }
}
