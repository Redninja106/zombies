using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal class Rotor : GameComponent
{
    public Rotor(float x, float y)
    {
        this.Transform.Position = new(x, y);
    }

    public override void Update()
    {
        this.Transform.Rotate(Time.DeltaTime * MathF.Tau * .05f);
    }
}
