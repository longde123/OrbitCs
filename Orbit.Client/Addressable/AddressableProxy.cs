using System.Reflection;
using Orbit.Client.Net;
using Orbit.Shared.Addressable;

namespace Orbit.Client.Addressable;

public class AddressableProxy<T> : DispatchProxy
    where T : class, IAddressable // T must be an interface
{
    private InvocationSystem _invocationSystem;

    private AddressableReference _reference;


    // but calls its Invoke method whenever an API is used).
    public static T Decorate(AddressableReference reference, InvocationSystem invocationSystem)
    {
        // DispatchProxy.Create creates proxy objects
        var proxy = Create<T, AddressableProxy<T>>()
            as AddressableProxy<T>;
        proxy.SetAddressableProxy(reference, invocationSystem);

        return proxy as T;
    }

    public void SetAddressableProxy(AddressableReference reference, InvocationSystem invocationSystem)
    {
        this._reference = reference;
        this._invocationSystem = invocationSystem;
    }


    protected override object? Invoke(MethodInfo? method, object?[]? args)
    {
        var mappedArgsWithType =
            args.Select((value, index) => { return Tuple.Create(value, method.GetParameters()[index].ParameterType); })
                .ToList();
        var invocation = new AddressableInvocation
        {
            Reference = _reference,
            Method = method.Name,
            Args = mappedArgsWithType
        };
        var completion = new Completion();
        _invocationSystem.SendInvocation(invocation, completion);

        if (method.ReturnType.IsGenericType)
        {
            var casting = completion.Task.CastTo(method.ReturnType.GenericTypeArguments[0]);

            return casting;
        }

        return completion.Task;
    }
}

public class AddressableProxyFactory
{
    private readonly InvocationSystem _invocationSystem;

    public AddressableProxyFactory(InvocationSystem invocationSystem)
    {
        this._invocationSystem = invocationSystem;
    }

    public T CreateProxy<T>(Type interfaceClass, Key key) where T : class, IAddressable
    {
        return AddressableProxy<T>.Decorate(new AddressableReference(interfaceClass.FullName, key), _invocationSystem);
    }
}

public static class TaskExtension
{
    public static Task CastTo(this Task task, Type targetType)
    {
        var taskType = task.GetType();
        var srcType = taskType.GetGenericArguments().First();
        var method = typeof(TaskExtension).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(m => m.Name == nameof(CastToImpl)).MakeGenericMethod(srcType, targetType);
        return (Task)method.Invoke(null, new[] { task });
    }

    private static Task CastToImpl<T, TR>(Task<T> task)
        where TR : T
    {
        var taskCompletionSource = new TaskCompletionSource<TR>();
        task.ContinueWith(t =>
        {
            // 异步执行失败处理
            if (t.IsFaulted)
                //todo
            {
                taskCompletionSource.TrySetException(t.Exception.InnerException);
            }
            // 异步被取消处理
            else if (t.IsCanceled)
            {
                taskCompletionSource.TrySetCanceled();
            }

            // 异步成功返回处理
            else
            {
                taskCompletionSource.TrySetResult((TR)t.Result);
            }
        });

        return taskCompletionSource.Task;
    }
}