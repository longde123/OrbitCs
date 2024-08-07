using System.Reflection;
using Orbit.Client.Util;
using AddressableType = System.String;
using AddressableInvocationArgument = System.Tuple<object, System.Type>;
using AddressableInvocationArguments = System.Collections.Generic.List<System.Tuple<object, System.Type>>;

namespace Orbit.Client.Addressable;

internal static class MethodInvoker
{
    public static async Task<object?> Invoke(
        object instance,
        string methodName,
        AddressableInvocationArguments args)
    {
        var argumentTypes = args.Select(arg => arg.Item2).ToArray();

        var method = MatchMethod(instance.GetType(), methodName, argumentTypes) ??
                     MatchMethod(instance.GetType(), methodName, argumentTypes.Concat(new[] { typeof(Task) }).ToArray())
                     ?? throw new NoSuchMethodException(
                         $"{methodName}({string.Join(", ", argumentTypes.Select(t => t.Name))})");

        var argumentValues = args.Select(arg => arg.Item1).ToArray();


        if (DeferredWrappers.IsAsync(method))
        {
            var result = await DeferredWrappers.WrapSuspend(method, instance, argumentValues);
            return result;
        }

        return method.Invoke(instance, argumentValues);
    }

    internal static MethodInfo? MatchMethod(Type clazz, string name, Type[] args)
    {
        return clazz.GetMethods().FirstOrDefault(m =>
            m.Name == name && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(args));
    }
}