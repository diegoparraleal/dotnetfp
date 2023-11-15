using System.Runtime.CompilerServices;

namespace DotnetFp.Core.Extensions;

public static class TupleExtensions
{
    #region AsEnumerable

    public static IEnumerable<T> AsEnumerable<T>(this (T, T) value) 
        => new[] { value.Item1, value.Item2 };
    public static IEnumerable<T> AsEnumerable<T>(this (T, T, T) value)
        => new[] { value.Item1, value.Item2, value.Item3 };
    public static IEnumerable<T> AsEnumerable<T>(this (T, T, T, T) value) 
        => new[] { value.Item1, value.Item2, value.Item3, value.Item4 };
    public static IEnumerable<T> AsEnumerable<T>(this (T, T, T, T, T) value) 
        => new[] { value.Item1, value.Item2, value.Item3, value.Item4, value.Item5 };

    #endregion

    #region GetAwaiter
    public static TaskAwaiter<(T1, T2)> GetAwaiter<T1, T2>(this(Task<T1>, Task<T2>) taskTuple)
    {
        async Task<(T1, T2)> TransposeAsync((Task<T1>, Task<T2>) tasks)
        {
            var (task1, task2) = tasks;
            await Task.WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }
        return TransposeAsync(taskTuple).GetAwaiter();
    }
    
    public static TaskAwaiter<(T1, T2, T3)> GetAwaiter<T1, T2, T3>(this(Task<T1>, Task<T2>, Task<T3>) taskTuple)
    {
        async Task<(T1, T2, T3)> TransposeAsync((Task<T1>, Task<T2>, Task<T3>) tasks)
        {
            var (task1, task2, task3) = tasks;
            await Task.WhenAll(task1, task2, task3);
            return (task1.Result, task2.Result, task3.Result);
        }
        return TransposeAsync(taskTuple).GetAwaiter();
    }
    
    public static TaskAwaiter<(T1, T2, T3, T4)> GetAwaiter<T1, T2, T3, T4>(this(Task<T1>, Task<T2>, Task<T3>, Task<T4>) taskTuple)
    {
        async Task<(T1, T2, T3, T4)> TransposeAsync((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks)
        {
            var (task1, task2, task3, task4) = tasks;
            await Task.WhenAll(task1, task2, task3, task4);
            return (task1.Result, task2.Result, task3.Result, task4.Result);
        }
        return TransposeAsync(taskTuple).GetAwaiter();
    }

    public static TaskAwaiter<(T1, T2, T3, T4, T5)> GetAwaiter<T1, T2, T3, T4, T5>(this(Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) taskTuple)
    {
        async Task<(T1, T2, T3, T4, T5)> TransposeAsync((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks)
        {
            var (task1, task2, task3, task4, task5) = tasks;
            await Task.WhenAll(task1, task2, task3, task4, task5);
            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result);
        }
        return TransposeAsync(taskTuple).GetAwaiter();
    }
    #endregion
}