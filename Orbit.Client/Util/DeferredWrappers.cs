/*
 Copyright (C) 2015 - 2019 Electronic Arts Inc.  All rights reserved.
 This file is part of the Orbit Project <https://www.orbit.cloud>.
 See license in LICENSE.
 */

using System.Reflection;
using System.Runtime.CompilerServices;

internal static class DeferredWrappers
{
    private static readonly List<Type> SupportedWrappers = new()
    {
        typeof(Task)
    };

    public static bool IsAsync(MethodInfo method)
    {
        return SupportedWrappers.Exists(wrapper => wrapper.IsAssignableFrom(method.ReturnType))
               || method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
    }

    public static async Task<object?> WrapSuspend(MethodInfo method, object target, object[] args = null)
    {
        args ??= Array.Empty<object>();
        //方法返回值
        var returnType = method.ReturnType;

        // 处理 Task 和 Task<> 异步方法调用
        if (returnType == typeof(Task) ||
            returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            // 调用方法并返回 Task 类型
            var task = (Task)method.Invoke(target, args);

            // 创建 TaskCompletionSource 实例，用于控制 Task 什么时候结束、取消、错误
            var taskCompletionSource = new TaskCompletionSource<object>();
            task.ContinueWith(t =>
            {
                // 异步执行失败处理
                if (t.IsFaulted)
                {
                    //todo
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
                    var result = returnType == typeof(Task) ? null : ((dynamic)t).Result;
                    taskCompletionSource.TrySetResult(result);
                }
            });

            return await taskCompletionSource.Task;
        }
        // 处理同步方法

        return method.Invoke(target, args);
    }
}