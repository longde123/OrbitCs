using System.Reflection;

namespace Orbit.Client.Reactive;

public static class AsyncObservableExtensions
{
    public static readonly Action<Exception> Throw = ex => { };

    public static void CastTo(this Task task, Type targetType, IAsyncObserver observer)
    {
        var taskType = task.GetType();
        var srcType = taskType.GetGenericArguments().First();
        var method = typeof(AsyncObservableExtensions).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(m => m.Name == nameof(CastToObserverImpl)).MakeGenericMethod(srcType, targetType);
        method.Invoke(null, new object[]
        {
            task, observer
        });
    }

    private static void CastToObserverImpl<T, TR>(Task<T> task, IAsyncObserver<TR> observer)
        where TR : T
    {
        task.ContinueWith(t =>
        {
            // 异步执行失败处理
            if (t.IsFaulted)
                //todo
            {
                observer.OnError(t.Exception.InnerException);
            }
            // 异步被取消处理
            else if (t.IsCanceled)
            {
            }

            // 异步成功返回处理
            else
            {
                observer.OnNext((TR)t.Result);
            }
        });
    }


    public static object CastTo(this IAsyncObserver<object> task, Type targetType)
    {
        var taskType = task.GetType();
        var srcType = taskType.GetGenericArguments().First();
        var method = typeof(AsyncObservableExtensions).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(m => m.Name == nameof(CastToActionImpl)).MakeGenericMethod(srcType, targetType);
        return method.Invoke(null, new[]
        {
            task
        });
    }

    private static AsyncSubscribe<TR> CastToActionImpl<T, TR>(IAsyncObserver<T> task)
        where TR : T
    {
        var taskCompletionSource = new AsyncSubscribe<TR>((result) => { task.OnNext(result); }, (error) => { task.OnError(error); });

        return taskCompletionSource;
    }
}