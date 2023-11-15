using DotnetFp.Core.Extensions;
using DotnetFp.Core.Monads;
using Newtonsoft.Json;

namespace DotnetFp.Core.Test;

public static class TestHelper
{
    public const string Default = "DEFAULT";
    public const string Something = "something";

    public static int MultiplyByTwo(int x) => x * 2;
    public static readonly IEnumerable<int> OneTwoSequence = new[] { 1, 2};
    public static readonly IEnumerable<int> Sequence = new[] { 1, 2, 3, 4, 5 };
    public static readonly IEnumerable<int> SequenceOneToTen = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public static readonly IEnumerable<int> SequenceTimesTwo = Sequence.Select(x => x * 2);
    public static Task<int> MultiplyByTwoAsync(int x) => (x * 2).AsTask();
    public static IEnumerable<int> NextSequence(int x)
    {
        var list = Sequence.ToList();
        return list.Select(y => (x-1) * list.Count + y);
    }

    public static Task<IEnumerable<int>> NextSequenceAsync(int x) => NextSequence(x).AsTask();
    public static string ToUpper(string x) => x.ToUpper();
    public static Maybe<string> MaybeToUpper(string x) => x.ToUpper().AsMaybe();
    public static Task<string> ToUpperAsync(string x) => x.ToUpper().AsTask();
    public static Task<Maybe<string>> MaybeToUpperAsync(string x) => MaybeToUpper(x).AsTask();
    public static Task<string> GenerateDefaultAsync() => Default.AsTask();
    public static Task<IEnumerable<int>> GenerateSequenceAsync() => Sequence.AsTask(); 
    public static Task<IEnumerable<int>> GenerateOneTwoSequenceAsync() => OneTwoSequence.AsTask(); 
    public static bool IsEven(int x) => x % 2 == 0; 
    public static Task<bool> IsEvenAsync(int x) => IsEven(x).AsTask(); 
}