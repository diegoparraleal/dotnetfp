namespace DotnetFp.Core;

public static class TaskExtensions
{
    public static Task<T> AsTask<T>(this T value)
        => Task.FromResult(value);
    public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> taskCollection)
        => await Task.WhenAll(taskCollection);
}