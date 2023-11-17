using DotnetFp.Core.Extensions;
using DotnetFp.Core.Models;
using DotnetFp.Core.Monads;
using Newtonsoft.Json;
using NUnit.Framework.Constraints;

namespace DotnetFp.Core.Test;

public static class TestHelper
{
    public const string Default = "DEFAULT";
    public const string NullStr = null!;
    public const string Something = "something";
    public const int Number = 1;
    public static readonly Error DefaultError = new Exception("error");
    public static readonly Issue DefaultIssue = Warning.Of("warning");
    
    public static readonly Maybe<string> MaybeStr = Maybe.Of(Something);
    public static readonly Maybe<string> Nothing = Maybe.Nothing<string>();
    public static readonly Expected<string> Success = Expected.Success(Something);
    public static readonly Expected<string> Failure = Expected.Failure<string>(DefaultError);
    public static readonly Validated<string> Valid = Validated.Valid(Something);
    public static readonly Validated<string> Invalid = Validated.Invalid<string>(DefaultIssue);
    public static readonly Choice<string, int> ChoiceStr = Choice.Of<string, int>(Something);
    public static readonly Choice<string, int> ChoiceInt = Choice.Of<string, int>(Number);
    public static readonly Pair<int> PairInt = Pair.Of(1, 2);

    public static int MultiplyByTwo(int x) => x * 2;
    public static readonly IEnumerable<int> OneTwoSequence = new[] { 1, 2};
    public static readonly IEnumerable<int> Sequence = new[] { 1, 2, 3, 4, 5 };
    public static readonly IEnumerable<int> SequenceOneToTen = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public static readonly IEnumerable<int> SequenceTimesTwo = Sequence.Select(x => x * 2);
    public static readonly Dictionary<int, string> Numbers = new() { {1, "One"}, {2, "Two"}};
    public static readonly IEnumerable<string> NumberTexts = new[] { "One", "Two"};
    public static Task<int> MultiplyByTwoAsync(int x) => (x * 2).AsTask();
    public static IEnumerable<int> NextSequence(int x)
    {
        var list = Sequence.ToList();
        return list.Select(y => (x-1) * list.Count + y);
    }

    public static Task<IEnumerable<int>> NextSequenceAsync(int x) => NextSequence(x).AsTask();
    public static string ToUpper(string x) => x.ToUpper();
    public static string ToUpperUnsafe(string x) => throw new Exception(DefaultError.Message);
    public static Maybe<string> MaybeToUpper(string x) => x.ToUpper().AsMaybe();
    public static Expected<string> ExpectedToUpper(string x) => x.ToUpper().AsExpected();
    public static Validated<string> ValidatedToUpper(string x) => x.ToUpper().AsValidated();
    public static Choice<string, int> ChoiceToUpper(string x) => Choice.Of<string, int>(x.ToUpper());
    public static Choice<string, int> ChoiceMultiplyByTwo(int x) => Choice.Of<string, int>(MultiplyByTwo(x));
    public static Pair<int> PairMultiplyByTwo(int x, int y) => Pair.Of(x * 2, y * 2);
    public static Task<string> ToUpperAsync(string x) => x.ToUpper().AsTask();
    public static Task<string> ToUpperUnsafeAsync(string x) => ToUpperUnsafe(x).AsTask();
    public static Task<Maybe<string>> MaybeToUpperAsync(string x) => MaybeToUpper(x).AsTask();
    public static Task<Expected<string>> ExpectedToUpperAsync(string x) => ExpectedToUpper(x).AsTask();
    public static Task<Validated<string>> ValidatedToUpperAsync(string x) => ValidatedToUpper(x).AsTask();
    public static Task<Choice<string, int>> ChoiceToUpperAsync(string x) => ChoiceToUpper(x).AsTask();
    public static Task<Choice<string, int>> ChoiceMultiplyByTwoAsync(int x) => ChoiceMultiplyByTwo(x).AsTask();
    public static Task<Pair<int>> PairMultiplyByTwoAsync(int x, int y) => Pair.Of(x * 2, y * 2).AsTask();
    public static Task<string> GenerateDefaultAsync() => Default.AsTask();
    public static Task<IEnumerable<int>> GenerateSequenceAsync() => Sequence.AsTask(); 
    public static Task<IEnumerable<int>> GenerateOneTwoSequenceAsync() => OneTwoSequence.AsTask(); 
    public static bool IsEven(int x) => x % 2 == 0; 
    public static Task<bool> IsEvenAsync(int x) => IsEven(x).AsTask();
    public static bool AssertThatAndReturnTrue<T>(T actual, IResolveConstraint expression) 
    {
        Assert.That(actual, expression);
        return true;
    }
}