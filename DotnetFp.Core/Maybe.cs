using System.Diagnostics.CodeAnalysis;
using DotnetFp.Core.Utils;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace DotnetFp.Core;

public readonly record struct Maybe<T>
{
    private const string None = "Nothing";
    private readonly T _value = default!;
    public bool HasValue { get; } 

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
    public Maybe<TR> Bind<TR>([NotNull] Func<T, Maybe<TR>> transformerFn) => HasValue ? transformerFn(_value) : Maybe<TR>.Nothing;
    public T OrElse(T defaultValue) => HasValue ? _value : defaultValue;
    public T OrThrow(string exceptionMessage = null) 
        => HasValue 
            ? _value 
            : throw new InvalidOperationException(exceptionMessage ?? $"A value of {typeof(T).PrettyName()} was expected.");
    public static Maybe<T> Nothing => default;
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
    public static T OrNull<T>(this Maybe<T> maybe) where T: class => maybe.OrElse(null);
}