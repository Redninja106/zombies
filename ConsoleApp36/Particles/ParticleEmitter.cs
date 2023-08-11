using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36.Particles;
internal class ParticleEmitter
{
    /// <summary>
    /// The rate at which attempts are made to emit particles, in attempts per second.
    /// </summary>
    public float Rate { get; set; }

    /// <summary>
    /// The probability of an attempt to emit a particle succeeding. Between 0 and 1, where 0 always fails and 1 always succeeds.
    /// </summary>
    public float Probability { get; set; }

    public Vector2 Velocity { get; set; }
    public float AngularVelocity { get; set; }

    private readonly ParticleSystem system;
    private float timeSinceLast = 0;

    internal ParticleEmitter(ParticleSystem system)
    {
        this.system = system;

        Rate = 0;
        Probability = 1;
    }

    public void Update()
    {
        timeSinceLast += Time.DeltaTime;

        var freq = (1f / Rate);
        while (timeSinceLast > freq)
        {
            timeSinceLast -= freq;

            if (system.Random.NextSingle() < Probability)
                system.EmitOne();
        }
    }

    public void Burst(int count, float duration = 0)
    {
        for (int i = 0; i < count; i++)
        {
            if (system.Random.NextSingle() < Probability)
                system.EmitOne();
        }
    }
}
