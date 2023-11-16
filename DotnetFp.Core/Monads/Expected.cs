using DotnetFp.Core.Extensions;
using DotnetFp.Core.Interfaces;
using DotnetFp.Core.Models;

namespace DotnetFp.Core.Monads;

public readonly record struct Expected<T>: IMonad<T>
{
    private readonly T _value;
    private readonly Error _error;
    private readonly bool _isSuccessful;

    private Expected(T value)
    {
        _value = value;
        _isSuccessful = true;
        _error = null;
    }
    
    private Expected(Error error)
    {
        _value = default;
        _isSuccessful = false;
        _error = error;
    }

    #region Monad functions

    public Expected<TR> Map<TR>(Func<T, TR> transformerFn)
        => _isSuccessful ? Expected.Success(transformerFn(_value)) : Expected.Failure<TR>(_error);
    
    public Expected<TR> Bind<TR>(Func<T, Expected<TR>> transformerFn)
        => _isSuccessful ? transformerFn(_value) : Expected.Failure<TR>(_error);
    #endregion

    #region Monad async functions
    public async Task<Expected<TR>> MapAsync<TR>(Func<T, Task<TR>> transformerFnAsync)
        => _isSuccessful ? Expected.Success(await transformerFnAsync(_value)) : Expected.Failure<TR>(_error);
    
    public async Task<Expected<TR>> BindAsync<TR>(Func<T, Task<Expected<TR>>> transformerFnAsync)
        => _isSuccessful ? await transformerFnAsync(_value) : Expected.Failure<TR>(_error);
    #endregion

    #region Downward functions to regular values
    public T OrFallback(T defaultValue) => _isSuccessful ? _value : defaultValue;
    public T OrFallback(Func<T> defaultValue) => _isSuccessful ? _value : defaultValue();
    public T OrRethrow() => _isSuccessful ? _value : throw _error.Exception;
    public Error ErrorOrThrow() => !_isSuccessful ? _error : throw new InvalidOperationException("Error was not set, this expected has a valid value");
    public TR Match<TR>(Func<T, TR> success, Func<Error, TR> error)
        => _isSuccessful ? success(_value) : error(_error);
    
    public bool IsSuccessful => _isSuccessful;
    
    public bool TryGet(out T value)
    {
        value = _value;
        return _isSuccessful;
    }
    
    public bool TryGetError(out Error error)
    {
        error = _error;
        return !_isSuccessful;
    }
    
    public void Deconstruct(out bool isSuccess, out T value, out Error error)
    {
        isSuccess = _isSuccessful;
        value = _value;
        error = _error;
    }
    #endregion

    #region Lifting functions

    public static Expected<T> Success(T value) => new(value);
    public static Expected<T> Failure(Error error) => new(error);

    #endregion
}

public static class Expected
{
    public static Expected<T> Success<T>(T value) => Expected<T>.Success(value);
    public static Expected<T> Failure<T>(Error error) => Expected<T>.Failure(error);
    public static bool IsSuccess<T>(Expected<T> expected) => expected.IsSuccessful;
    public static bool IsFailure<T>(Expected<T> expected) => !expected.IsSuccessful;
}

public static class Try
{
    public static Expected<T> Run<T>(Func<T> callbackFn)
    {
        try { return Expected.Success(callbackFn()); }
        catch (Exception exception) { return Expected.Failure<T>(exception); }
    }
    
    public static async Task<Expected<T>> RunAsync<T>(Func<Task<T>> callbackFn)
    {
        try { return Expected.Success(await callbackFn()); }
        catch (Exception exception)  { return Expected.Failure<T>(exception); }
    }
}

public static class ExpectedExtensions
{
    #region Maybe related extensions
    public static Maybe<T> AsMaybe<T>(this Expected<T> expected) 
        => expected.Match(success: Maybe.Of, error: _ => Maybe.Nothing<T>());
    #endregion
    
    #region IEnumerable related extensions
    public static IEnumerable<T> AsEnumerable<T>(this Expected<T> expected)
    {
        if (expected.TryGet(out var value)) yield return value;
    }
    
    public static IEnumerable<T> SelectValidValues<T>(this IEnumerable<Expected<T>> collection)
        => collection
            .WhereSuccessful()
            .Select(x => x.OrRethrow());
    
    public static IEnumerable<Error> SelectErrors<T>(this IEnumerable<Expected<T>> collection)
        => collection
            .WhereFailure()
            .Select(x => x.ErrorOrThrow());
    
    public static IEnumerable<Expected<T>> WhereSuccessful<T>(this IEnumerable<Expected<T>> collection)
        => collection.Where(Expected.IsSuccess);
    
    public static IEnumerable<Expected<T>> WhereFailure<T>(this IEnumerable<Expected<T>> collection)
        => collection.Where(Expected.IsFailure);
    
    public static (IEnumerable<T>, IEnumerable<Error>) Partition<T>(this IEnumerable<Expected<T>> collection)
        => collection
            .Partition(Expected.IsSuccess)
            .Map(x => (
                x.True.Select(y => y.OrRethrow()), 
                x.False.Select( y => y.ErrorOrThrow()))
            );
    #endregion
}