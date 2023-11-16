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
    private readonly bool _hasValue;

    private Maybe([NotNull] T value)
    {
        _value = value.ThrowIfNull();
        _hasValue = true;
    }
    
    #region Monad functions
    public Maybe<TR> Map<TR>([NotNull] Func<T, TR> transformerFn) => _hasValue ? Maybe.Of(transformerFn(_value)) : Maybe<TR>.Nothing; 
    public Maybe<TR> Bind<TR>([NotNull] Func<T, Maybe<TR>> transformerFn) => _hasValue ? transformerFn(_value) : Maybe<TR>.Nothing;
    #endregion

    #region Monad async functions
    public async Task<Maybe<TR>> MapAsync<TR>([NotNull] Func<T, Task<TR>> transformerFn) 
        => _hasValue ? Maybe.Of(await transformerFn(_value)) : Maybe<TR>.Nothing;
    public async Task<Maybe<TR>> BindAsync<TR> ([NotNull] Func<T, Task<Maybe<TR>>> transformerFn) 
        => _hasValue ? await transformerFn(_value) : Maybe<TR>.Nothing;
    #endregion

    #region Downward functions to regular values
    public T OrElse(T defaultValue) => _hasValue ? _value : defaultValue;
    public T OrElse(Func<T> defaultValueFn) => _hasValue ? _value : defaultValueFn();
    public async Task<T> OrElseAsync(Func<Task<T>> defaultValueFn) => _hasValue ? _value : await defaultValueFn();
    public T OrThrow(string exceptionMessage = null) 
        => _hasValue ? _value : throw new InvalidOperationException(exceptionMessage ?? $"A value of {typeof(T).PrettyName()} was expected.");
    
    public bool HasValue => _hasValue;
    
    public bool TryGet(out T value)
    {
        value = _value;
        return _hasValue;
    }
    
    public void Deconstruct(out bool hasValue, out T value)
    {
        hasValue = _hasValue;
        value = _value;
    }
    #endregion
    
    #region Lifting functions
    public static Maybe<T> Nothing => default;
    public static Maybe<TR> Of<TR>(TR value) => new(value);

    public static implicit operator Maybe<T>(T value) => Of(value);
    #endregion
    
    public override string ToString() => $"({(_hasValue ? _value.ToString() : None)})";
}

public static class Maybe
{
    public static Maybe<T> Of<T>(T value) => Maybe<T>.Of(value);
    public static Maybe<T> Nothing<T>() => Maybe<T>.Nothing;
    public static bool HasValue<T>(Maybe<T> maybe) => maybe.HasValue;
    public static bool IsNothing<T>(Maybe<T> maybe) => !maybe.HasValue;
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

    public static IEnumerable<T> SelectValues<T>(this IEnumerable<Maybe<T>> collection)
        => collection
            .WhereHasValue()
            .Select(x => x.OrThrow());
    
    public static IEnumerable<Maybe<T>> WhereHasValue<T>(this IEnumerable<Maybe<T>> collection)
        => collection.Where(Maybe.HasValue);
    
    public static IEnumerable<Maybe<T>> WhereNothing<T>(this IEnumerable<Maybe<T>> collection)
        => collection.Where(Maybe.IsNothing);

    #endregion
}