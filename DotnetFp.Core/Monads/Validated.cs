using DotnetFp.Core.Extensions;
using DotnetFp.Core.Interfaces;
using DotnetFp.Core.Models;

namespace DotnetFp.Core.Monads;

public readonly struct Validated<T>: IMonad<T>, IEquatable<Validated<T>>
{
    private readonly T _value;
    private readonly List<Issue> _issues;
    private readonly bool _isValid;

    private Validated(T value)
    {
        _value = value;
        _isValid = true;
        _issues = null;
    }
    
    private Validated(IReadOnlyCollection<Issue> issues)
    {
        _value = default;
        _isValid = false;
        _issues = issues.ToList();
    }
    
    private Validated(Issue issue)
    {
        _value = default;
        _isValid = false;
        _issues = new List<Issue> { issue };
    }

    #region Monad functions

    public Validated<TR> Map<TR>(Func<T, TR> transformerFn)
        => _isValid ? Validated.Valid(transformerFn(_value)) : Validated.Invalid<TR>(_issues);
    
    public Validated<TR> Bind<TR>(Func<T, Validated<TR>> transformerFn)
        => _isValid ? transformerFn(_value) : Validated.Invalid<TR>(_issues);
    #endregion

    #region Monad async functions
    public async Task<Validated<TR>> MapAsync<TR>(Func<T, Task<TR>> transformerFnAsync)
        => _isValid ? Validated.Valid(await transformerFnAsync(_value)) : Validated.Invalid<TR>(_issues);
    
    public async Task<Validated<TR>> BindAsync<TR>(Func<T, Task<Validated<TR>>> transformerFnAsync)
        => _isValid ? await transformerFnAsync(_value) : Validated.Invalid<TR>(_issues);
    #endregion

    #region Downward functions to regular values
    public T OrFallback(T defaultValue) => _isValid ? _value : defaultValue;
    public T OrFallback(Func<T> defaultValue) => _isValid ? _value : defaultValue();
    public TR Match<TR>(Func<T, TR> valid, Func<IEnumerable<Issue>, TR> invalid)
        => _isValid ? valid(_value) : invalid(_issues);
    
    public T OrThrow() => _isValid ? _value : throw new InvalidOperationException($"Found issues: {AllIssues}");
    public IEnumerable<Issue> IssuesOrThrow() => !_isValid ? _issues : throw new InvalidOperationException("Validated does not contain issues");
    
    public bool IsValid => _isValid;
    
    public bool TryGet(out T value)
    {
        value = _value;
        return _isValid;
    }
    
    public bool TryGetIssues(out IEnumerable<Issue> issues)
    {
        issues = _issues;
        return !_isValid;
    }
    
    public void Deconstruct(out bool isValid, out T value, out IReadOnlyCollection<Issue> issues)
    {
        isValid = _isValid;
        value = _value;
        issues = _issues;
    }
    #endregion

    #region Lifting functions
    public static Validated<T> Valid(T value) => new(value);
    public static Validated<T> Invalid(Issue issue) => new(issue);
    public static Validated<T> Invalid(IReadOnlyCollection<Issue> issues) => new(issues);
    public static implicit operator Validated<T>(T value) => Valid(value);
    public static implicit operator Validated<T>(Issue issue) => Invalid(issue);
    #endregion

    public Validated<T> AddIssue(Issue issue)
    {
        var issues = new List<Issue>();
        if (_issues != null ) issues.AddRange(_issues);
        issues.Add(issue);
        return Validated.Invalid<T>(issues);
    }

    public string AllIssues => _issues?.Join();
    
    public override string ToString() => $"({(_isValid ? _value.ToString() : $"Issues: {AllIssues}")})";

    #region Equality members
    public bool Equals(Validated<T> other) 
        => EqualityComparer<T>.Default.Equals(_value, other._value) && 
           _issues.ToEmptyIfNull()
               .Zip(other._issues.ToEmptyIfNull())
               .All(x => x.First.Equals(x.Second)) &&
           _isValid == other._isValid;

    public override bool Equals(object obj) 
        => obj is Validated<T> other && 
           Equals(other);

    public override int GetHashCode() => HashCode.Combine(_value, _issues, _isValid);

    public static bool operator ==(Validated<T> left, Validated<T> right) => left.Equals(right);

    public static bool operator !=(Validated<T> left, Validated<T> right) => !left.Equals(right);
    #endregion
}

public static class Validated
{
    public static Validated<T> Valid<T>(T value) => Validated<T>.Valid(value);
    public static Validated<T> Invalid<T>(Issue issue) => Validated<T>.Invalid(issue);
    public static Validated<T> Invalid<T>(IReadOnlyCollection<Issue> issues) => Validated<T>.Invalid(issues);
    public static Validated<T> Invalid<T>(params Issue[] issues) => Validated<T>.Invalid(issues);
    public static Validated<T> FromException<T>(Exception exception) => Validated<T>.Invalid((Error) exception);
    public static bool IsValid<T>(Validated<T> validated) => validated.IsValid;
    public static bool IsInvalid<T>(Validated<T> validated) => !validated.IsValid;
}

public static class ValidatedExtensions
{
    public static Validated<T> AsValidated<T>(this T value) => value;
    public static Validated<T> AsValidationIssue<T>(this Issue issue) => issue;
    public static Validated<T> AsValidationIssues<T>(this IReadOnlyCollection<Issue> issues) => Validated<T>.Invalid(issues);
    
    #region Maybe related extensions
    public static Maybe<T> AsMaybe<T>(this Validated<T> validated) 
        => validated.Match(valid: Maybe.Of, invalid: _ => Maybe.Nothing<T>());
    #endregion
    
    #region Expected related extensions
    private static Expected<T> Failure<T>(IEnumerable<Issue> issues) 
        => Expected<T>.Failure(new InvalidOperationException(issues.Join()));
    
    public static Expected<T> AsExpected<T>(this Validated<T> validated) 
        => validated.Match(valid: Expected.Success, invalid: Failure<T>);
    #endregion
    
    #region IEnumerable related extensions
    public static IEnumerable<T> AsEnumerable<T>(this Validated<T> validated)
    {
        if (validated.TryGet(out var value)) yield return value;
    }
    
    public static IEnumerable<T> SelectValidValues<T>(this IEnumerable<Validated<T>> collection)
        => collection
            .WhereValid()
            .Select(x => x.OrThrow());
    
    public static IEnumerable<Issue> SelectErrors<T>(this IEnumerable<Validated<T>> collection)
        => collection
            .WhereInvalid()
            .SelectMany(x => x.IssuesOrThrow());
    
    public static IEnumerable<Validated<T>> WhereValid<T>(this IEnumerable<Validated<T>> collection)
        => collection.Where(Validated.IsValid);
    
    public static IEnumerable<Validated<T>> WhereInvalid<T>(this IEnumerable<Validated<T>> collection)
        => collection.Where(Validated.IsInvalid);
    
    public static (IEnumerable<T>, IEnumerable<Issue>) Partition<T>(this IEnumerable<Validated<T>> collection)
        => collection
            .Partition(Validated.IsValid)
            .Map(x => (
                x.True.Select(y => y.OrThrow()), 
                x.False.SelectMany( y => y.IssuesOrThrow()))
            );
    #endregion
}