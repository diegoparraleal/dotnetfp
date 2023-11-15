using System.Diagnostics.CodeAnalysis;
using DotnetFp.Core.Extensions;
using DotnetFp.Core.Interfaces;
using DotnetFp.Core.Utils;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace DotnetFp.Core.Monads;

public readonly record struct Maybe<T>: IMonad<T>
{
    private const string None = "Nothing";
    private readonly T _value = default!;
    public bool HasValue { get; } 
    public static Maybe<T> Nothing => default;

    internal Maybe([NotNull] T value)
    {
        _value = value.ThrowIfNull();
        HasValue = true;
    }
    
    public bool TryGet(out T value)
    {
        value = _value;
        return HasValue;
    }

    public Maybe<TR> Map<TR>([NotNull] Func<T, TR> transformerFn) => HasValue ? Maybe.Of(transformerFn(_value)) : Maybe<TR>.Nothing; 
    public async Task<Maybe<TR>> MapAsync<TR>([NotNull] Func<T, Task<TR>> transformerFn) 
        => HasValue ? Maybe.Of(await transformerFn(_value)) : Maybe<TR>.Nothing;
    public Maybe<TR> Bind<TR>([NotNull] Func<T, Maybe<TR>> transformerFn) => HasValue ? transformerFn(_value) : Maybe<TR>.Nothing;
    public async Task<Maybe<TR>> BindAsync<TR> ([NotNull] Func<T, Task<Maybe<TR>>> transformerFn) 
        => HasValue ? await transformerFn(_value) : Maybe<TR>.Nothing;
    public T OrElse(T defaultValue) => HasValue ? _value : defaultValue;
    public T OrElse(Func<T> defaultValueFn) => HasValue ? _value : defaultValueFn();
    public async Task<T> OrElseAsync(Func<Task<T>> defaultValueFn) => HasValue ? _value : await defaultValueFn();
    public T OrThrow(string exceptionMessage = null) 
        => HasValue 
            ? _value 
            : throw new InvalidOperationException(exceptionMessage ?? $"A value of {typeof(T).PrettyName()} was expected.");
    public override string ToString() => $"({(HasValue ? _value.ToString() : None)})";

    public void Deconstruct(out bool hasValue, out T value)
    {
        hasValue = HasValue;
        value = _value;
    }
}

public static class Maybe
{
    public static Maybe<TR> Of<TR>(TR value) => new (value);
}

public static class MaybeExtensions
{
    public static Maybe<T> AsMaybe<T>(this T value) => Maybe.Of(value);
    public static T OrNull<T>(this Maybe<T> maybe) where T: class => maybe.OrElse((T)null);
    
    #region IEnumerable related extensions
    public static IEnumerable<T> AsEnumerable<T>(this Maybe<T> maybe)
    {
        if (maybe.TryGet(out var value)) yield return value;
    }
    #endregion
}