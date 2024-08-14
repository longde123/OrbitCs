using System.Collections.Concurrent;

namespace Orbit.Util.Di;

public class ComponentContainer
{
    private readonly ConcurrentDictionary<Type, Registration> _registry = new();

    private readonly ConcurrentDictionary<Type, object> _registryCache = new();

    public ComponentContainer()
    {
        Register<ComponentContainer>(() => this);
    }

    public void Register<T>(Func<object> factory)
    {
        var type = typeof(T);
        Register(type, _ => factory.Invoke());
    }

    public void Register(Type type, Func<ComponentContainer, object> factory)
    {
        var registration = new Registration(type, factory);
        _registry.TryRemove(type, out _);
        _registry[type] = registration;
    }

    public T Resolve<T>(Type type)
    {
        if (_registry.TryGetValue(type, out var registration))
        {
            if (_registryCache.TryGetValue(type, out var registrationValue))
            {
                return (T)registrationValue;
            }

            var obj = ((Func<ComponentContainer, object>)registration.Factory).Invoke(this);
            _registryCache.TryAdd(type, obj);
            return (T)obj;
        }

        return Construct<T>(type);
    }

    public T Construct<T>(Type concreteClass)
    {
        var constructors = concreteClass.GetConstructors();
        var ctr = constructors[0];
        if (constructors.Length > 1 && ctr.GetParameters().Length == 0)
        {
            ctr = constructors[1];
            //throw new InvalidOperationException($"{concreteClass.Name} must have one constructor.");
        }

        var args = new object[ctr.GetParameters().Length];
        for (var i = 0; i < ctr.GetParameters().Length; i++)
        {
            args[i] = Resolve<object>(ctr.GetParameters()[i].ParameterType);
        }

        return (T)ctr.Invoke(args);
    }

    public T Construct<T>()
    {
        return Construct<T>(typeof(T));
    }

    public TR Resolve<TR>()
    {
        return Resolve<TR>(typeof(TR));
    }

    public Lazy<TR> Inject<TR>()
    {
        return new Lazy<TR>(() => Resolve<TR>());
    }

    public void Configure(Action config)
    {
        Configure(_ => config.Invoke());
    }

    public void Configure(Action<ComponentContainerRoot> config)
    {
        config(new ComponentContainerRoot(this));
    }

    public class Registration
    {
        public Registration(Type type, Func<ComponentContainer, object> factory)
        {
            Type = type;
            Factory = factory;
        }

        public Type Type { get; }
        public Func<ComponentContainer, object> Factory { get; }
    }
}

public class ComponentContainerRoot
{
    private readonly ComponentContainer _container;

    public ComponentContainerRoot(ComponentContainer container)
    {
        _container = container;
    }

    public void Register<T>(Func<ComponentContainer, T> body)
    {
        _container.Register(typeof(T), c => body(c));
    }

    public void Register<T>(Func<T> body)
    {
        _container.Register(typeof(T), _ => body());
    }

    public void Singleton<T>(Type clazz)
    {
        if (clazz == null)
        {
            throw new Exception("clazz in null");
        }

        _container.Register(typeof(T), _ => _container.Construct<T>(clazz));
    }

    public void Singleton<T>()
    {
        Singleton<T>(typeof(T));
    }

    public void Instance<T>(T instance)
    {
        _container.Register(typeof(T), _ => instance);
        // container.Resolve<T>();
    }

    public void ExternallyConfigured<T>(ExternallyConfigured<T> config)
    {
        Singleton<T>(config.InstanceType);
        _container.Register(config.GetType(), _ => config);
    }

    public T Resolve<T>()
    {
        return _container.Resolve<T>();
    }
}

public class ExternallyConfigured<T>
{
    public virtual Type InstanceType => null;
}