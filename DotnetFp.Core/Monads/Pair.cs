using System.Diagnostics.CodeAnalysis;
using DotnetFp.Core.Extensions;
using DotnetFp.Core.Interfaces;

namespace DotnetFp.Core.Monads;

public readonly record struct Pair<T>: IMonad<T>
{
    private readonly T _first;
    private readonly T _second;

    private Pair([NotNull] T first, [NotNull] T second)
    {
        _first = first;
        _second = second;
    }

    #region Monad functions

    public Pair<TR> Map<TR>(Func<T, TR> transformFn) => Pair.Of(transformFn(_first), transformFn(_second));
    public Pair<TR> Bind<TR>(Func<T, T, Pair<TR>> transformFn) => transformFn(_first, _second);
    
    #endregion

    #region Monad async functions
    public async Task<Pair<TR>> MapAsync<TR>(Func<T, Task<TR>> transformFnAsync)
    {
        var (t1, t2) = await (transformFnAsync(_first), transformFnAsync(_second));
        return Pair.Of(t1, t2);
    }

    public async Task<Pair<TR>> BindAsync<TR>(Func<T, T, Task<Pair<TR>>> transformFnAsync) => await transformFnAsync(_first, _second);

    #endregion

    #region Downward functions to regular values

    public T First => _first;
    public T Second => _second;

    public void Deconstruct(out T first, out T second)
    {
        first = _first;
        second = _second;
    }

    #endregion

    #region Lifting functions

    public static Pair<T> Of(T first, T second) => new(first, second);
    
    public static implicit operator Pair<T>((T, T) value) => Of(value.Item1, value.Item2);
    #endregion
    
    public override string ToString() => $"({_first}, {_second})";
}

public static class Pair
{
    public static Pair<T> Of<T>(T first, T second) => Pair<T>.Of(first, second);
}

public static class PairExtensions
{
    public static Pair<T> AsPair<T>(this (T, T) value) => Pair.Of(value.Item1, value.Item2);
    public static (T, T) AsTuple<T>(this Pair<T> pair) => (pair.First, pair.Second);
    
    #region IEnumerable related extensions
    public static IEnumerable<T> AsEnumerable<T>(this Pair<T> pair)
    {
        yield return pair.First;
        yield return pair.Second;
    }
    
    public static IEnumerable<T> SelectMany<T>(this Pair<IEnumerable<T>> pair) 
        => pair.AsEnumerable().SelectMany(x => x);

    #endregion
}