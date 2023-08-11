using ConsoleApp36.Particles;
using SimulationFramework.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;

internal class Player : GameComponent, ICollider
{
    float zoom = 0;
    private readonly PlayerHands hands;
    private readonly Vector2[] collider;
    private ITexture soldier;
    private readonly PlayerItemList playerItemList;
    private Vector2 velocity;

    public float AngularVelocity { get; private set; }
    public Vector2 Velocity => velocity;
    public float Mass => 1f;
    public ColliderKind Kind => ColliderKind.Dynamic;
    public override RenderLayer RenderLayer => RenderLayer.Entities;

    public PlayerItemList ItemList => playerItemList;

    public ReadOnlySpan<Vector2> GetPolygon() => collider;

    public Player()
    {
        soldier = Graphics.LoadTexture("Assets/soldier1_gun.png");
        hands = Program.World.Register<PlayerHands>(new(this));
        collider = PolygonUtils.CreateEllipse(16, .35f, .5f);
        playerItemList = Program.World.Register(new PlayerItemList());
    }

    public override void Render(ICanvas canvas)
    {
        base.Render(canvas);
        canvas.Translate(.1f, 0);
        canvas.DrawTexture(soldier, new Rectangle(0, 0, 1, 1, Alignment.Center));
    }

    public override void Update()
    {
        var abilities = playerItemList.GetPlayerAbilities();

        var camera = Program.Camera;
        var mousePosition = camera.ScreenToWorld(Mouse.Position);
        var targetAngle = Angle.FromVector(mousePosition - Transform.Position);
        var turnSpeed = Time.DeltaTime * 540 * Angle.DegreesToRadians;
        var oldRot = this.Transform.Rotation;
        this.Transform.Rotation = Angle.Step(this.Transform.Rotation, targetAngle, turnSpeed);
        this.AngularVelocity = this.Transform.Rotation - oldRot;

        camera.Transform.Position = Vector2.Lerp(camera.Transform.Position, this.Transform.Position, MathUtils.TimescaledLerpFactor(15, Time.DeltaTime));

        zoom += Mouse.ScrollWheelDelta;
        camera.VerticalSize = 20 * MathF.Pow(1.1f, -zoom);

        Vector2 movementVector = Vector2.Zero;
        if (Keyboard.IsKeyDown(Key.W))
            movementVector -= Vector2.UnitY;
        if (Keyboard.IsKeyDown(Key.S))
            movementVector += Vector2.UnitY;
        if (Keyboard.IsKeyDown(Key.A))
            movementVector -= Vector2.UnitX;
        if (Keyboard.IsKeyDown(Key.D))
            movementVector += Vector2.UnitX;

        velocity = velocity.StepTowards(movementVector * abilities.MoveSpeed, abilities.Acceleration * Time.DeltaTime);

        this.Transform.Translate(velocity * Time.DeltaTime);

        if (Keyboard.IsKeyPressed(Key.Z) || Keyboard.IsKeyDown(Key.X))
        {
            Program.World.Register(new Zombie(mousePosition.X, mousePosition.Y));
        }

        if (Keyboard.IsKeyPressed(Key.Key1))
        {
            playerItemList.AddItem(SpeedBoostItem.Instance);
        }

        if (Keyboard.IsKeyPressed(Key.Key2))
        {
            playerItemList.AddItem(DamageItem.Instance);
        }

        // if (Program.Collision.RayCast(Transform.Position, Transform.Right, 100, c => c != this, out RayCastHit hit))
        // {
        //     target = hit.Point;
        // }
        // else
        // {
        //     target = Transform.Right * 100;
        // }
    }

    public void OnCollision(ICollider other, Vector2 mtv)
    {
    }

}

internal class PlayerHands : GameComponent
{
    public Weapon EquippedWeapon { get; private set; }
    public WeaponState PrimaryWeaponState { get; private set; }
    public WeaponState MeleeWeaponState { get; private set; }
    public Player Player { get; }

    private ParticleSystem muzzleFlash;

    public PlayerHands(Player player) : base(player.Transform)
    {
        EquippedWeapon = new PistolWeapon();
        this.Player = player;

        muzzleFlash = World.Register(new ParticleSystem(75, this.Transform)
            .WithState<ParticleTransformBehavior>(s => new(s.Transform, new(.5f, .225f)))
            .WithState<ParticleVelocityBehavior>(s => new(s.Emitter.Velocity + Angle.ToVector(s.Transform.WorldRotation + s.Random.NextSingle(-.3f, .3f)) * s.Random.NextSingle(7f, 10f), s.Random.NextSingle(MathF.Tau)))
            .WithState<ParticleLifetimeBehavior>(s => new(s.Random.NextSingle(.05f, .1f)))
            .WithState<RenderRectangleBehavior>(s => new(.1f, Color.FromHSV(s.Random.NextSingle(0, 1 / 6f), 1f, .8f, s.Random.NextSingle(.6f, .8f))))
            .WithState<ParticleDragBehavior>()
            );

        MeleeWeaponState = World.Register(new MeleeWeaponState(this.Transform));
    }

    public override void Render(ICanvas canvas)
    {
        base.Render(canvas);
    }

    public override void Update()
    {
        var abilities = Player.ItemList.GetPlayerAbilities();

        MouseButton fireButton = MouseButton.Left;
        bool firing = EquippedWeapon.IsSemiAuto ? Mouse.IsButtonPressed(fireButton) : Mouse.IsButtonDown(fireButton);

        if (firing)
        {
            Vector2 barrelVelocity = Player.Velocity + CentrifugalForce(new(.5f, .225f), Vector2.Zero, Player.AngularVelocity);
            muzzleFlash.Emitter.Velocity = barrelVelocity;
            muzzleFlash.Emitter.Burst(15);
            var barrelPos = this.Transform.LocalToWorld(new(1f, .225f));
            var mousePos = Program.Camera.ScreenToWorld(Mouse.Position);
            var bullet = new Bullet(abilities.Damage, barrelPos, Angle.FromVector(mousePos - barrelPos), barrelVelocity);
            Program.World.Register(bullet);
        }

        if (MeleeWeaponState.IsReady && Mouse.IsButtonDown(MouseButton.Right))
        {
            MeleeWeaponState.Activate();
        }
    }

    private Vector2 CentrifugalForce(Vector2 point, Vector2 center, float angularVelocity)
    {
        var diff = point - center;
        Vector2 perpendicular = new(diff.Y, -diff.X);
        return perpendicular * angularVelocity;
    }
}

abstract class WeaponState : GameComponent
{
    public virtual float CooldownRemaining { get; protected set; }

    public virtual bool IsReady => CooldownRemaining <= 0;

    public WeaponState(Transform? parent = null) : base(parent)
    {
    }

    public abstract void Activate();
}

class MeleeWeapon
{
    public float SwingTime { get; set; } = .5f;
    public float SweepAngle { get; set; } = 150f * Angle.DegreesToRadians;
}

class MeleeWeaponState : WeaponState
{
    private float? swingProgress;
    MeleeWeapon weapon = new();
    Axe axe;

    public MeleeWeaponState(Transform parent) : base(parent)
    {
        axe = World.Register(new Axe(this.Transform));
        axe.Transform.Position = new(.2f, -.2f);
    }

    public override void Activate()
    {
        CooldownRemaining = 1;
        swingProgress = 0;
        axe.BeginSwing();
    }

    public override void Update()
    {
        if (swingProgress is null)
        {
            if (CooldownRemaining > 0)
                CooldownRemaining -= Time.DeltaTime;
            
            return;
        }

        if (swingProgress > 1)
        {
            swingProgress = null;
            axe.Swinging = false;
        }
        else
        {
            var speed = weapon.SweepAngle / weapon.SwingTime;
            swingProgress += speed * Time.DeltaTime;

            axe.Transform.Position = new(.2f, -.2f);
            var x = MathF.Pow(swingProgress.Value, 1.5f);
            axe.Transform.Rotation = x * weapon.SweepAngle;
            axe.Swinging = true;
        }
    }

    public override void Render(ICanvas canvas)
    {
        base.Render(canvas);
    }
}

class Axe : GameComponent, ICollider
{
    private ITexture axeTexture = Graphics.LoadTexture("Assets/axe.png");
    public ColliderKind Kind => ColliderKind.Ghost;
    public float Mass => 0;
    public RenderLayer RenderLayer => 0;
    public bool Swinging = false;

    private readonly HashSet<IDamagable> hit = new();
    private Vector2[] collider;
    public ReadOnlySpan<Vector2> GetPolygon() => collider;

    public Axe(Transform parent) : base(parent)
    {
    }

    public void BeginSwing()
    {
        hit.Clear();
    }

    public void OnCollision(ICollider other, Vector2 mtv)
    {
        if (Swinging && other is IDamagable damagable && !hit.Contains(damagable))
        {
            damagable.Damage(2.5f, DamageKind.Slash);
            hit.Add(damagable);
        }
    }

    public override void Render(ICanvas canvas)
    {
        if (!Swinging)
            return;

        base.Render(canvas);
        canvas.Translate(.18f, 0);
        canvas.Rotate(-18 * Angle.DegreesToRadians);
        canvas.Scale(1f / (axeTexture.Width * 2));
        canvas.Scale(1.25f);
        canvas.DrawTexture(axeTexture, Alignment.BottomCenter);
    }

    public override void Update()
    {
        Rectangle rectangle = new(0, -.5f, .3f, .35f, Alignment.BottomCenter);
        collider = PolygonUtils.CreateRect(rectangle);
    }
}
