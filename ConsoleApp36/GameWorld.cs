using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp36;
internal class GameWorld
{
    private readonly List<IGameComponent> componentsInUpdateOrder = new();
    private readonly List<IGameComponent> componentsInRenderOrder = new();

    private readonly Queue<IGameComponent> componentsToAdd = new();
    private readonly Queue<IGameComponent> componentsToRemove = new();

    private bool componentsLocked = false;

    private static readonly IComparer<IGameComponent> renderLayerComparer = Comparer<IGameComponent>.Create((x, y) =>
    {
        var result = x.RenderLayer - y.RenderLayer;
        
        if (result is 0)
            return x.GetHashCode() - y.GetHashCode();
        
        return result;
    });

    public GameWorld()
    {

    }

    public TComponent Register<TComponent>(TComponent component) where TComponent : IGameComponent
    {
        if (componentsLocked)
        {
            componentsToAdd.Enqueue(component);
        }
        else
        {
            AddComponentCore(component);
        }

        return component;
    }

    private void AddComponentCore(IGameComponent component)
    {
        componentsInUpdateOrder.Add(component);
        componentsInUpdateOrder.Sort(renderLayerComparer);

        componentsInRenderOrder.Add(component);
        componentsInRenderOrder.Sort(renderLayerComparer);
    }

    private void RemoveComponentCore(IGameComponent component)
    {
        componentsInUpdateOrder.Remove(component);
        componentsInRenderOrder.Remove(component);
    }

    private void ClearLockQueues()
    {
        while (componentsToAdd.TryDequeue(out var c))
            AddComponentCore(c);

        while (componentsToRemove.TryDequeue(out var c))
            RemoveComponentCore(c);
    }

    public void Update()
    {
        ClearLockQueues();
        componentsLocked = true;
        foreach (var c in componentsInUpdateOrder)
        {
            c.Update();
        }
        componentsLocked = false;
    }

    public void Render(ICanvas canvas)
    {
        ClearLockQueues();

        componentsLocked = true;
        foreach (var c in componentsInRenderOrder)
        {
            canvas.PushState();
            c.Render(canvas); 
            canvas.PopState();
        };
        componentsLocked = false;

        // render unlit background
        // render occludable objects to texture
        // render lights, fill with occludable texture
    }

    public TComponent Find<TComponent>() where TComponent : IGameComponent
    {
        return Find<TComponent>(_ => true);
    }

    public TComponent Find<TComponent>(Predicate<TComponent> predicate) where TComponent : IGameComponent
    {
        return componentsInUpdateOrder.OfType<TComponent>().First(c => predicate(c));
    }

    public IEnumerable<IGameComponent> FindAll()
    {
        return FindAll(c => true);
    }

    public IEnumerable<IGameComponent> FindAll(Predicate<IGameComponent> predicate)
    {
        return FindAll<IGameComponent>(predicate);
    }

    public IEnumerable<TComponent> FindAll<TComponent>() where TComponent : IGameComponent
    {
        return FindAll<TComponent>(c => true);
    }

    public IEnumerable<TComponent> FindAll<TComponent>(Predicate<TComponent> predicate) where TComponent : IGameComponent
    {
        return componentsInUpdateOrder.OfType<TComponent>().Where(c => predicate(c));
    }

    public void Destroy(IGameComponent component)
    {
        if (componentsLocked)
        {
            componentsToRemove.Enqueue(component);
        }
        else
        {
            RemoveComponentCore(component);
        }
    }
}
