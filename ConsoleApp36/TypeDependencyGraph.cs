using System.Collections;
using System.Reflection;

namespace ConsoleApp36.Particles;

class TypeDependencyGraph : IEnumerable<Type>
{
    private readonly List<Type> types = new();
    private readonly Dictionary<Type, DependencyGraphAttribute[]> attributes = new();

    public IEnumerator<Type> GetEnumerator() => types.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public TypeDependencyGraph()
    {
    }

    public void Add(Type type, IEnumerable<DependencyGraphAttribute>? attributes = null)
    {
        attributes ??= type.GetCustomAttributes<DependencyGraphAttribute>();
        this.attributes.Add(type, attributes.ToArray());

        int i;
        for (i = 0; i < types.Count; i++)
        {
            if (DependsOn(types[i], type))
            {
                break;
            }
        }

        types.Insert(i, type);
    }

    public void Remove(Type type)
    {
        types.RemoveAll(v => v == type);
        attributes.Remove(type);
    }

    private bool DependsOn(Type type, Type dependency)
    {
        if (attributes[type].OfType<DependsOnAttribute>().Any(a => a.DependencyType == dependency))
            return true;

        if (attributes[dependency].OfType<DependencyOfAttribute>().Any(a => a.DependentType == type))
            return true;

        return false;
    }
}