using Microsoft.Extensions.Logging;
using Orbit.Util.Time;

namespace Orbit.Client.Addressable;

using AddressableClass = Type;

public class CapabilitiesScanner
{
    private readonly Clock _clock;
    private readonly OrbitClientConfig _config;
    private readonly ILogger<CapabilitiesScanner> _logger;

    private readonly List<string> _packagePaths;

    public CapabilitiesScanner(Clock clock, OrbitClientConfig config, ILoggerFactory loggerFactory)
    {
        _clock = clock;
        _config = config;
        _packagePaths = config.Packages.ToList();
        _logger = loggerFactory.CreateLogger<CapabilitiesScanner>();
    }

    public List<AddressableClass> AddressableInterfaces { get; private set; }
    public List<AddressableClass> AddressableClasses { get; private set; }
    public Dictionary<AddressableClass, AddressableClass> InterfaceLookup { get; private set; }


    public void Scan()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(ass => ass.GetTypes()
                .Where(t => _packagePaths.Contains(t.Namespace)))
            .ToList();
        ScanTypes(types);
    }


    private static bool IsAddressableInterfaces(Type t)
    {
        return typeof(IAddressable).IsAssignableFrom(t) && t.IsInterface;
    }

    private static bool IsAddressableClasses(Type t)
    {
        return typeof(IAddressable).IsAssignableFrom(t) && t.IsClass;
    }

    private static bool IsHasAnnotationNonConcrete(Type interfaceClass)
    {
        return interfaceClass.IsDefined(typeof(NonConcrete), false);
    }

    public void ScanTypes(List<Type> types)
    {
        _logger.LogInformation("Scanning for node capabilities...");
        var stopwatch = Stopwatch.Start(_clock);


        AddressableInterfaces = types
            .Where(it => IsAddressableInterfaces(it) && !IsHasAnnotationNonConcrete(it))
            .Select(it => { return it; })
            .ToList();

        AddressableClasses = types
            .Where(it => IsAddressableClasses(it) && !IsHasAnnotationNonConcrete(it))
            .Where(it => !it.IsAbstract)
            .Select(it => { return it; })
            .ToList();

        InterfaceLookup = new Dictionary<AddressableClass, AddressableClass>();
        foreach (var implClass in AddressableClasses)
        {
            var mapped = ResolveMapping(implClass);
            if (!mapped.Any())
            {
                throw new InvalidOperationException($"Could not find mapping for {implClass.Name}");
            }

            foreach (var iface in mapped)
            {
                if (InterfaceLookup.ContainsKey(iface))
                {
                    throw new InvalidOperationException(
                        $"Multiple implementations of concrete interface {iface.Name} found.");
                }

                InterfaceLookup[iface] = implClass;
            }
        }


        var elapsed = stopwatch.Elapsed;
        _logger.LogDebug("Addressable Interfaces: {0}", string.Join(", ", AddressableInterfaces));
        _logger.LogDebug("Addressable Classes: {0}", string.Join(", ", AddressableClasses));
        _logger.LogDebug("Implemented Addressables: {0}",
            string.Join(", ", InterfaceLookup.Select(kv => $"{kv.Key} => {kv.Value}")));
        var interfacesNoImplemented = AddressableInterfaces.Except(InterfaceLookup.Keys);
        _logger.LogDebug("Addressable Interfaces No Implemented: {0}", string.Join(", ", interfacesNoImplemented));

        _logger.LogInformation(
            "Node capabilities scan complete in {0}ms. {1} implemented addressable(s) found. {2} addressable interface(s) found. {3} addressable class(es) found.",
            elapsed, InterfaceLookup.Count, AddressableInterfaces.Count, AddressableClasses.Count);
    }

    private ICollection<Type> ResolveMapping(Type crawl, List<Type> list = null)
    {
        if (list == null)
        {
            list = new List<Type>();
        }

        if (crawl.GetInterfaces().Length == 0)
        {
            return list;
        }

        foreach (var iface in crawl.GetInterfaces())
        {
            if (IsAddressableInterfaces(iface))
            {
                if (!IsHasAnnotationNonConcrete(iface))
                {
                    list.Add(iface);
                }

                if (iface.GetInterfaces().Length > 0)
                {
                    ResolveMapping(iface, list);
                }
            }
        }

        return list;
    }
}