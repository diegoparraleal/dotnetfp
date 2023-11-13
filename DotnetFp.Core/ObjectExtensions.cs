namespace DotnetFp.Core;

public static class ObjectExtensions
{
    public static T ApplyIf<T>(this T value, bool condition, Func<T> generatorFn) => condition ? generatorFn() : value;
    public static T WhenNullThen<T>(this T value, Func<T> generatorFn) => value.ApplyIf(value is null, generatorFn);
    public static T WhenNullThen<T>(this T value, T defaultValue) => value.ApplyIf(value is null, () => defaultValue);
    public static IEnumerable<T> Once<T>(this T value) => new[] { value };
}