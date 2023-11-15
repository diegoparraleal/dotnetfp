using System.Diagnostics.CodeAnalysis;

namespace DotnetFp.Core.Extensions;

public static class EnumerableExtensions
{
    #region Async related

    public static async Task<IEnumerable<TR>> Select<T, TR>(
        [NotNull] this Task<IEnumerable<T>> collectionTask, 
        [NotNull] Func<T, TR> transformFn)
        => (await collectionTask).Select(transformFn);
    
    public static async Task<IEnumerable<TR>> SelectAsync<T, TR>(
        [NotNull] this IEnumerable<T> collection, 
        [NotNull] Func<T, Task<TR>> transformFn)
        => await collection.Select(transformFn).WhenAll();
    
    public static async Task<IEnumerable<TR>> SelectMany<T, TR>(
        [NotNull] this Task<IEnumerable<T>> collectionTask, 
        [NotNull] Func<T, IEnumerable<TR>> transformFn)
        => (await collectionTask).SelectMany(transformFn);
    
    public static async Task<IEnumerable<TR>> SelectManyAsync<T, TR>(
        [NotNull] this IEnumerable<T> collection, 
        [NotNull] Func<T, Task<IEnumerable<TR>>> transformFn)
        => (await collection.Select(transformFn).WhenAll())
            .SelectMany(x => x);

    public static IEnumerable<(T Item, TR Projection)> Project<T, TR>(
        [NotNull] this IEnumerable<T> collection,
        [NotNull] Func<T, TR> transformFn) => collection.Select(x => (x, transformFn(x)));
    
    public static Task<IEnumerable<(T Item, TR Projection)>> ProjectAsync<T, TR>(
        [NotNull] this IEnumerable<T> collection,
        [NotNull] Func<T, Task<TR>> transformFn)
    {
        async Task<(T, TR)> TransformFnAsync(T t) => (t, await transformFn(t));
        return collection.SelectAsync(TransformFnAsync);
    }

    public static async Task<IEnumerable<T>> Where<T>(
        [NotNull] this Task<IEnumerable<T>> collectionTask, 
        [NotNull] Func<T, bool> predicateFn)
        => (await collectionTask).Where(predicateFn);
    
    public static async Task<IEnumerable<T>> WhereAsync<T>(
        [NotNull] this IEnumerable<T> collection, 
        [NotNull] Func<T, Task<bool>> predicateFn) 
        => await collection.ProjectAsync(predicateFn)
            .Where(x => x.Projection)
            .Select(x => x.Item);

    #endregion
}