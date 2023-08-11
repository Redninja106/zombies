using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal abstract class Weapon
{
    public abstract bool IsSemiAuto { get; }

    public abstract void Fire();
}

class PistolWeapon : Weapon
{
    public override bool IsSemiAuto => true;

    public override void Fire()
    {
    }
}