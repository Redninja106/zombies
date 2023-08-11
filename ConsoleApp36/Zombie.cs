using ConsoleApp36.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;

internal class Zombie : GameComponent, ICollider, IDamagable
{
    private static ITexture? zombieTexture;
    private static Vector2[]? collider;
    private float health = 5;
    private ParticleSystem bloodSystem;
    private ParticleSystem bloodTrail;

    public override RenderLayer RenderLayer => RenderLayer.Entities;
    public ColliderKind Kind => ColliderKind.Dynamic;
    public float Mass => 1;

    public Zombie(float x, float y)
    {
        bloodSystem = World.Register(
            new ParticleSystem(50, this.Transform)
            .WithState<ParticleTransformBehavior>(s => new(s.Transform.WorldPosition + s.Random.NextUnitVector2() * s.Random.NextSingle(.5f), s.Transform.WorldRotation))
            .WithState<ParticleVelocityBehavior>(s => new(-s.Emitter.Velocity + s.Random.NextUnitVector2() * s.Random.NextSingle(), s.Random.NextSingle() * MathF.Tau))
            .WithState<ParticleLifetimeBehavior>(s => new(s.Random.NextSingle() * .5f + .5f))
            .WithState<ParticleDragBehavior>()
            .WithState<RenderRectangleBehavior>(s => new(s.Random.NextSingle() * .05f + .05f, Color.FromHSV(1, 1, 1, s.Random.NextSingle(.2f, .3f))))
            );

        bloodTrail = World.Register(
            new ParticleSystem(50, Transform, renderLayer: RenderLayer.Decals)
            .WithState<ParticleTransformBehavior>(s => new(s.Transform, new(0, s.Random.NextSingle(-.5f, .5f))))
            .WithState<RenderRectangleBehavior>(s => new(s.Random.NextSingle(.1f, .3f), Color.FromHSV(1, 1, 1, s.Random.NextSingle(.2f, .3f))))
            .WithState<OpacityOverTimeBehavior>()
            .WithState<DestroyOldestAtCapacityBehavior>()
            );

        bloodTrail.Emitter.Rate = 5;
        bloodTrail.Emitter.Probability = .01f;

        zombieTexture ??= Graphics.LoadTexture("Assets/zoimbie1_hold.png");
        this.Transform.Position = new(x, y);
    }

    struct OpacityOverTimeBehavior : IParticleBehavior    {
        public float change;

        public OpacityOverTimeBehavior(float changeInOpacity)
        {
            this.change = changeInOpacity;
        }

        public void Render(ICanvas canvas, Particle particle)
        {
        }

        public void Update(Particle particle)
        {
            ref var opacity = ref particle.GetBehavior<RenderRectangleBehavior>();
        }
    }

    public override void Update()
    {
        var player = Program.World.Find<Player>();
        float speed = 2;
        this.Transform.TurnTowards(player.Transform.Position, speed * Time.DeltaTime);
        this.Transform.Translate(this.Transform.Right * Time.DeltaTime, false);
    }

    public override void Render(ICanvas canvas)
    {
        base.Render(canvas);
        canvas.DrawTexture(zombieTexture!, new Rectangle(0, 0, 1, 1, Alignment.Center));
    }

    public ReadOnlySpan<Vector2> GetPolygon()
    {
        return collider ??= PolygonUtils.CreateEllipse(5, .4f, .5f);
    }

    public void OnCollision(ICollider other, Vector2 mtv)
    {
    }

    public void Damage(float damage, DamageKind kind)
    {
        bloodSystem.Emitter.Burst(10);
        bloodTrail.Emitter.Burst(100);

        health -= damage;
        if (health <= 0)
        {
            if (Random.Shared.NextSingle() > .2f)
                World.Register(new ItemPickup(this.Transform.Position, Random.Shared.NextSingle() >= .5f ? SpeedBoostItem.Instance : DamageItem.Instance, 1));

            this.bloodTrail.Transform.Reparent(null);
            this.bloodTrail.Emitter.Rate = 0;
            World.Destroy(this);
        }
    }
}