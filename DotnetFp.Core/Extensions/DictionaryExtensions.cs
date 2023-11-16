using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Extensions;

public static class DictionaryExtensions
{
    public static Maybe<TValue> MaybeGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        => dictionary.TryGetValue(key, out var value) ? value : Maybe.Nothing<TValue>();
    
    public static TValue MaybeGetOrElse<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        => dictionary.TryGetValue(key, out var value) ? value : defaultValue;
}