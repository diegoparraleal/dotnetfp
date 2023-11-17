using System.Diagnostics.CodeAnalysis;
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
    
    public T ValueOrThrow<T>()
        => typeof(T) == typeof(TL) ? (T) Convert.ChangeType(_left, typeof(T)) 
            : typeof(T) == typeof(TR) ? (T) Convert.ChangeType(_right, typeof(T))
                : throw new InvalidOperationException($"Generic type should be of {typeof(TL).PrettyName()} or {typeof(TR).PrettyName()}");

    public bool IsLeft => _isLeft;
    public bool IsRight => _isRight;

    public bool Is<T>()
        => typeof(T) == typeof(TL) ? _isLeft
            : typeof(T) == typeof(TR) ? _isRight
               : throw new InvalidOperationException($"Generic type should be of {typeof(TL).PrettyName()} or {typeof(TR).PrettyName()}");
    
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
    
    public static implicit operator Choice<TL, TR>(TL value) => Of(value);
    public static implicit operator Choice<TL, TR>(TR value) => Of(value);
    #endregion
    
    public override string ToString() => $"({(_isLeft ? _left : _right)})";
    
    private Choice<TLResult, TR> ChangeLeftType<TLResult>() => Choice<TLResult, TR>.Of(_right);  
    private Choice<TL, TRResult> ChangeRightType<TRResult>() => Choice<TL, TRResult>.Of(_left);  
}

public static class Choice
{
    public static Choice<TL, TR> Of<TL, TR>(TL value) => Choice<TL, TR>.Of(value);
    public static Choice<TL, TR> Of<TL, TR>(TR value) => Choice<TL, TR>.Of(value);
    public static bool IsLeft<TL, TR>(Choice<TL, TR> choice) => choice.IsLeft;
    public static bool IsRight<TL, TR>(Choice<TL, TR> choice) => choice.IsRight;
}

public static class ChoiceExtensions
{
    public static Choice<TL, TR> AsChoice<TL, TR>(this TL value) => value;
    public static Choice<TL, TR> AsChoice<TL, TR>(this TR value) => value;
    
    #region IEnumerable related extensions
    public static IEnumerable<TL> LeftAsEnumerable<TL, TR>(this Choice<TL, TR> choice) 
    {
        if (choice.IsLeft) yield return choice.LeftOrThrow();
    }
    
    public static IEnumerable<TR> RightAsEnumerable<TL, TR>(this Choice<TL, TR> choice) 
    {
        if (choice.IsRight) yield return choice.RightOrThrow();
    }
    
    public static IEnumerable<Choice<TL, TR>> WhereLeft<TL, TR>(this IEnumerable<Choice<TL, TR>> collection)
        => collection.Where(Choice.IsLeft);
    
    public static IEnumerable<Choice<TL, TR>> WhereRight<TL, TR>(this IEnumerable<Choice<TL, TR>> collection)
        => collection.Where(Choice.IsRight);
    
    public static IEnumerable<TL> SelectLeftValues<TL, TR>(this IEnumerable<Choice<TL, TR>> collection) 
        => collection
            .WhereLeft()
            .Select(x => x.LeftOrThrow());
    
    public static IEnumerable<TR> SelectRightValues<TL, TR>(this IEnumerable<Choice<TL, TR>> collection) 
        => collection
            .WhereRight()
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