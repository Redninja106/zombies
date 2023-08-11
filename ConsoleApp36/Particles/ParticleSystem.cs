using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36.Particles;
internal sealed class ParticleSystem : GameComponent 
{
    private readonly Dictionary<Type, IParticleBehaviorContainer> states = new();
    private readonly TypeDependencyGraph updateGraph = new();
    private readonly int maxParticles;
    private readonly Random random;
    
    private int activeParticles = 0;

    public override RenderLayer RenderLayer { get; }

    public ParticleEmitter Emitter { get; }
    public Random Random => random;

    public bool IsFull => activeParticles == maxParticles;

    public ParticleSystem(int maxParticles, Transform? parent = null, Random? random = null, RenderLayer renderLayer = RenderLayer.Particles) : base(parent)
    {
        this.random = random ?? new();
        this.maxParticles = maxParticles;
        this.Emitter = new(this);
        this.RenderLayer = renderLayer;
    }

    public override void Update()
    {
        foreach (var type in updateGraph)
        {
            var state = states[type];
            state.Update(activeParticles);
        }

        Emitter.Update();
    }

    public override void Render(ICanvas canvas)
    {
        foreach (var type in updateGraph)
        {
            var state = states[type];
            state.Render(canvas, activeParticles);
        }
    }

    public ParticleSystem WithState<TState>(Func<ParticleSystem, TState>? stateProvider = null)
        where TState : IParticleBehavior
    {
        states.Add(typeof(TState), new ParticleStateContainer<TState>(this, maxParticles, stateProvider));

        updateGraph.Add(
            typeof(TState),
            typeof(TState)
                .GetMethod(nameof(IParticleBehavior.Update))!
                .GetCustomAttributes<DependencyGraphAttribute>()
            );

        return this;
    }

    public ParticleStateContainer<TBehavior> GetBehaviorContainer<TBehavior>()
        where TBehavior : IParticleBehavior
    {
        return (ParticleStateContainer<TBehavior>)states[typeof(TBehavior)];
    }

    public void EmitOne()
    {
        if (activeParticles == maxParticles)
        {
            return;
        }

        // position particle immediately after currently active ones
        var index = activeParticles++;

        foreach (var (_, state) in states)
        {
            state.Initialize(index, random);
        }
    }

    public void Remove(int index)
    {
        var lastActive = activeParticles - 1;

        if (lastActive != index)
        {
            foreach (var (_, state) in states)
            {
                state.Swap(index, lastActive);
            }
        }

        activeParticles--;
    }
}