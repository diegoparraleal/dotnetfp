using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using DotnetFp.Core.Extensions;
using DotnetFp.Core.Interfaces;

namespace DotnetFp.Core.Monads;

public readonly record struct Choice<TL, TR>: IMonad<TL, TR>
{
    private readonly TL _left;
    private readonly TR _right;
    private readonly bool _isLeft;
    private readonly bool _isRight;

    private Choice([NotNull] TL value)
    {
        _left = value;
        _right = default;
        _isLeft = true;
        _isRight = false;
    }

    private Choice([NotNull] TR value)
    {
        _left = default;
        _right = value;
        _isLeft = false;
        _isRight = true;
    }

    #region Monad functions
    public Choice<TLResult, TR> MapLeft<TLResult>([NotNull] Func<TL, TLResult> leftFn)
        => _isLeft ? Choice<TLResult, TR>.Of(leftFn(_left)) : Choice<TLResult, TR>.Of(_right);
    
    public Choice<TL, TRResult> MapRight<TRResult>([NotNull] Func<TR, TRResult> rightFn)
        => _isLeft ? Choice<TL, TRResult>.Of(_left) : Choice<TL, TRResult>.Of(rightFn(_right));
    
    public Choice<TLResult, TRResult> Map<TLResult, TRResult>(
        [NotNull] Func<TL, TLResult> leftFn, 
        [NotNull] Func<TR, TRResult> rightFn)
        => _isLeft ? MapLeft(leftFn).ChangeRightType<TRResult>(): MapRight(rightFn).ChangeLeftType<TLResult>();
    
    public Choice<TLResult, TR> BindLeft<TLResult>([NotNull] Func<TL, Choice<TLResult, TR>> leftFn)
        => _isLeft ? leftFn(_left) : Choice<TLResult, TR>.Of(_right);
    
    public Choice<TL, TRResult> BindRight<TRResult>([NotNull] Func<TR, Choice<TL, TRResult>> rightFn)
        => _isLeft ? Choice<TL, TRResult>.Of(_left) : rightFn(_right);
    
    public Choice<TLResult, TRResult> Bind<TLResult, TRResult>(
        [NotNull] Func<TL, Choice<TLResult, TR>> leftFn,
        [NotNull] Func<TR, Choice<TL, TRResult>> rightFn)
        => _isLeft ? BindLeft(leftFn).ChangeRightType<TRResult>() : BindRight(rightFn).ChangeLeftType<TLResult>();
    #endregion
    
    #region Monad async functions
    public async Task<Choice<TLResult, TR>> MapLeftAsync<TLResult>([NotNull] Func<TL, Task<TLResult>> leftFnAsync)
        => _isLeft ? Choice<TLResult, TR>.Of(await leftFnAsync(_left)) : Choice<TLResult, TR>.Of(_right);
    
    public async Task<Choice<TL, TRResult>> MapRightAsync<TRResult>([NotNull] Func<TR, Task<TRResult>> rightFnAsync)
        => _isLeft ? Choice<TL, TRResult>.Of(_left) : Choice<TL, TRResult>.Of(await rightFnAsync(_right));
    
    public async Task<Choice<TLResult, TRResult>> MapAsync<TLResult, TRResult>(
        [NotNull] Func<TL, Task<TLResult>> leftFnAsync, 
        [NotNull] Func<TR, Task<TRResult>> rightFnAsync)
        => _isLeft 
            ? (await MapLeftAsync(leftFnAsync)).ChangeRightType<TRResult>()
            : (await MapRightAsync(rightFnAsync)).ChangeLeftType<TLResult>();
    
    public async Task<Choice<TLResult, TR>> BindLeftAsync<TLResult>([NotNull] Func<TL, Task<Choice<TLResult, TR>>> leftFnAsync)
        => _isLeft ? await leftFnAsync(_left) : Choice<TLResult, TR>.Of(_right);
    
    public async Task<Choice<TL, TRResult>> BindRightAsync<TRResult>([NotNull] Func<TR, Task<Choice<TL, TRResult>>> rightFnAsync)
        => _isLeft ? Choice<TL, TRResult>.Of(_left) : await rightFnAsync(_right);
    
    public async Task<Choice<TLResult, TRResult>> BindAsync<TLResult, TRResult>(
        [NotNull] Func<TL, Task<Choice<TLResult, TR>>> leftFnAsync,
        [NotNull] Func<TR, Task<Choice<TL, TRResult>>> rightFnAsync)
        => _isLeft 
            ? (await BindLeftAsync(leftFnAsync)).ChangeRightType<TRResult>() 
            : (await BindRightAsync(rightFnAsync)).ChangeLeftType<TLResult>();
    #endregion
    
    #region Downward functions to regular values
    public T Match<T>(Func<TL, T> left, Func<TR, T> right)
        => _isLeft ? left(_left) : right(_right);
    
    public TL LeftOrThrow(string exceptionMessage = null)
        => _isLeft ? _left : throw new InvalidOperationException(exceptionMessage ?? $"Left value of {typeof(TL).PrettyName()} was expected.");
    
    public TR RightOrThrow(string exceptionMessage = null)
        => _isRight ? _right : throw new InvalidOperationException(exceptionMessage ?? $"Left value of {typeof(TL).PrettyName()} was expected.");

    public T ValueOrThrow<T>(string exceptionMessage = null) 
        => typeof(T) switch
        {
            TL => (T)(object) _left,
            TR => (T)(object) _right,
            _ => throw new InvalidOperationException($"Generic type should be of {typeof(TL).PrettyName()} or {typeof(TR).PrettyName()}")
        };

    public bool IsLeft => _isLeft;
    public bool IsRight => _isRight;
    public bool Is<T>()
        => typeof(T) switch
        {
            TL => _isLeft,
            TR => _isRight,
            _ => throw new InvalidOperationException($"Generic type should be of {typeof(TL).PrettyName()} or {typeof(TR).PrettyName()}")
        }; 
    
    public bool TryGet(out TL value)
    {
        value = _left;
        return _isLeft;
    }
    
    public bool TryGet(out TR value)
    {
        value = _right;
        return _isRight;
    }
    
    public void Deconstruct(out bool isLeft, out bool isRight, out TL left, out TR right)
    {
        isLeft = _isLeft;
        isRight = _isRight;
        left = _left;
        right = _right;
    }
    #endregion

    #region Lifting functions
    public static Choice<TL, TR> Of(TL value) => new(value);
    public static Choice<TL, TR> Of(TR value) => new(value);
    #endregion
    
    private Choice<TLResult, TR> ChangeLeftType<TLResult>() => Choice<TLResult, TR>.Of(_right);  
    private Choice<TL, TRResult> ChangeRightType<TRResult>() => Choice<TL, TRResult>.Of(_left);  
}

public static class Choice
{
    public static bool Is<TL, TR, T>(Choice<TL, TR> choice) => choice.Is<T>();
    public static bool IsLeft<TL, TR>(Choice<TL, TR> choice) => choice.IsLeft;
    public static bool IsRight<TL, TR>(Choice<TL, TR> choice) => choice.IsRight;
}

public static class ChoiceExtensions
{
    #region IEnumerable related extensions
    public static IEnumerable<TBase> AsEnumerable<TL, TR, TBase>(this Choice<TL, TR> choice) 
        where TL: TBase where TR: TBase
    {
        yield return choice.Match(left: x => (TBase) x, right: x => x);
    }

    public static IEnumerable<TL> SelectLeftValues<TL, TR>(this IEnumerable<Choice<TL, TR>> collection) 
        => collection
            .Where(Choice.IsLeft)
            .Select(x => x.LeftOrThrow());
    
    public static IEnumerable<TR> SelectRightValues<TL, TR>(this IEnumerable<Choice<TL, TR>> collection) 
        => collection
            .Where(Choice.IsRight)
            .Select(x => x.RightOrThrow());
    
    public static (IEnumerable<TL>, IEnumerable<TR>) Partition<TL, TR>(this IEnumerable<Choice<TL, TR>> collection)
        => collection
            .Partition(Choice.IsLeft)
            .Map(x => (
                x.True.Select(y => y.LeftOrThrow()), 
                x.False.Select( y => y.RightOrThrow()))
            );

    #endregion
}