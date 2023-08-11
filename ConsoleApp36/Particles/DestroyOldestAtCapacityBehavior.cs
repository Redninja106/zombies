using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36.Particles;

struct DestroyOldestAtCapacityBehavior : IParticleBehavior
{
    private float age;

    public void Render(ICanvas canvas, Particle particle)
    {
    }

    public void Update(Particle particle)
    {
        var container = particle.System.GetBehaviorContainer<DestroyOldestAtCapacityBehavior>();
        var state = (State)(container.SharedState ??= new State());

        if (particle.System.IsFull)
        {
            if (age >= state.lastHighestAge)
            {
                particle.Destroy();
            }
        }

        age += Time.DeltaTime;

        if (age > state.highestAge)
        {
            state.highestAge = age;
        }
    }

    private sealed class State : IParticleBehaviorState
    {
        public float lastHighestAge;
        public float highestAge;

        public void Render(ICanvas canvas)
        {
        }

        public void Update()
        {
            lastHighestAge = highestAge;
            highestAge = 0;
        }
    }
}
