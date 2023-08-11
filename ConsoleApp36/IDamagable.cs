using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal interface IDamagable
{
    void Damage(float damage, DamageKind kind);
}

enum DamageKind
{
    Bullet,
    Slash,
}
