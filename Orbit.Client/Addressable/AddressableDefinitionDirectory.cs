using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Orbit.Shared.Mesh;
using AddressableClass = System.Type;

namespace Orbit.Client.Addressable;

public class AddressableDefinitionDirectory
{
    private readonly ConcurrentDictionary<AddressableClass, AddressableImplDefinition>
        _implDefinitionMap = new();

    private readonly ConcurrentDictionary<AddressableClass, AddressableInterfaceDefinition>
        _interfaceDefinitionMap = new();

    private readonly ConcurrentDictionary<(AddressableClass, AddressableClass), AddressableImplDefinition>
        _interfaceImplComboImplDefinitionMap = new();

    private readonly ILogger<AddressableDefinitionDirectory> _logger;


    public AddressableDefinitionDirectory(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AddressableDefinitionDirectory>();
    }

    public void SetupDefinition(IEnumerable<AddressableClass> interfaceClasses,
        Dictionary<AddressableClass, AddressableClass> impls)
    {
        foreach (var interfaceClass in interfaceClasses)
        {
            GetOrCreateInterfaceDefinition(interfaceClass);
        }

        foreach (var pair in impls)
        {
            var interfaceClass = pair.Key;
            var implClass = pair.Value;
            var implDefinition = GenerateImplDefinition(interfaceClass, implClass);
            _implDefinitionMap[interfaceClass] = implDefinition;
        }
    }

    public AddressableInterfaceDefinition GetOrCreateInterfaceDefinition(AddressableClass interfaceClass)
    {
        return _interfaceDefinitionMap.GetOrAdd(interfaceClass, GenerateInterfaceDefinition);
    }

    public AddressableImplDefinition GetImplDefinition(string name)
    {
        //todo 优化


        var implDefinition = _implDefinitionMap.FirstOrDefault(idm => idm.Key.FullName == name);
        if (null == implDefinition.Key)
        {
            throw new InvalidOperationException($"No implementation found for {name}");
        }

        return GetImplDefinition(implDefinition.Key);
    }

    public AddressableImplDefinition GetImplDefinition(AddressableClass interfaceClass)
    {
        if (!_implDefinitionMap.TryGetValue(interfaceClass, out var implDefinition))
        {
            throw new InvalidOperationException($"No implementation found for {interfaceClass}");
        }

        return implDefinition;
    }

    public AddressableImplDefinition OnDemandImplClass(AddressableClass interfaceClass, AddressableClass implClass)
    {
        return _interfaceImplComboImplDefinitionMap.GetOrAdd((interfaceClass, implClass),
            _ => GenerateImplDefinition(interfaceClass, implClass));
    }

    private AddressableInterfaceDefinition GenerateInterfaceDefinition(AddressableClass interfaceClass)
    {
        if (!interfaceClass.IsInterface)
        {
            throw new ArgumentException($"{interfaceClass.Name} is not an interface.");
        }

        if (interfaceClass.IsDefined(typeof(NonConcrete), false))
        {
            throw new ArgumentException($"{interfaceClass.Name} is non-concrete and can not be directly addressed");
        }

        var methods = interfaceClass.GetMethods()
            .ToDictionary(method => method, GenerateInterfaceMethodDefinition);

        var definition = new AddressableInterfaceDefinition
        {
            InterfaceClass = interfaceClass,
            Methods = methods
        };

        _logger.LogDebug($"Created interface definition: {definition}");

        return definition;
    }

    private AddressableInterfaceMethodDefinition GenerateInterfaceMethodDefinition(MethodInfo method)
    {
        VerifyMethodIsAsync(method);

        return new AddressableInterfaceMethodDefinition
        {
            Method = method
        };
    }

    private AddressableImplDefinition GenerateImplDefinition(AddressableClass interfaceClass,
        AddressableClass implClass)
    {
        var interfaceDef = GetOrCreateInterfaceDefinition(interfaceClass);

        var methods = implClass.GetMethods()
            .ToDictionary(method => method, GenerateImplMethodDefinition);

        var onActivateMethod = methods.Values.SingleOrDefault(method => method.IsOnActivate);
        var onDeactivateMethod = methods.Values.SingleOrDefault(method => method.IsOnDeactivate);

        var definition = new AddressableImplDefinition
        {
            InterfaceClass = interfaceClass,
            ImplClass = implClass,
            InterfaceDefinition = interfaceDef,
            Methods = methods,
            OnActivateMethod = onActivateMethod,
            OnDeactivateMethod = onDeactivateMethod
        };

        _logger.LogDebug($"Created implementation definition: {definition}");

        return definition;
    }

    private AddressableImplMethodDefinition GenerateImplMethodDefinition(MethodInfo method)
    {
        var isOnActivate = method.IsDefined(typeof(OnActivate), false);
        var isOnDeactivate = method.IsDefined(typeof(OnDeactivate), false);

        if (isOnActivate || isOnDeactivate)
        {
            VerifyMethodIsAsync(method);
        }

        return new AddressableImplMethodDefinition
        {
            Method = method,
            IsOnActivate = isOnActivate,
            IsOnDeactivate = isOnDeactivate
        };
    }

    private void VerifyMethodIsAsync(MethodInfo method)
    {
        if (!DeferredWrappers.IsAsync(method))
        {
            throw new ArgumentException($"Method {method} does not return asynchronous type.");
        }
    }

    public NodeCapabilities GenerateCapabilities()
    {
        return new NodeCapabilities(
            _interfaceDefinitionMap.Keys.Select(key => key.FullName).ToHashSet()
        );
    }
}