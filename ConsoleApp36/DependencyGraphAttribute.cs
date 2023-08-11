namespace ConsoleApp36;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
abstract class DependencyGraphAttribute : Attribute
{

}

class DependencyOfAttribute : DependencyGraphAttribute
{
    public Type DependentType { get; }

    public DependencyOfAttribute(Type dependentType)
    {
        DependentType = dependentType;
    }
}

class DependencyOfAttribute<T> : DependencyOfAttribute
{
    public DependencyOfAttribute() : base(typeof(T))
    {
    }
}

class DependsOnAttribute<T> : DependsOnAttribute
{
    public DependsOnAttribute() : base(typeof(T))
    {
    }
}

class DependsOnAttribute : DependencyGraphAttribute
{
    public Type DependencyType { get; }

    public DependsOnAttribute(Type dependencyType)
    {
        DependencyType = dependencyType;
    }
}
