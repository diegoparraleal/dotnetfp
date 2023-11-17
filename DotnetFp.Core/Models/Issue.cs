using DotnetFp.Core.Extensions;

namespace DotnetFp.Core.Models;

public enum IssueType
{
    Error,
    Warning
}

public abstract class Issue: IEquatable<Issue>
{
    protected Issue(string message, IssueType issueType)
    {
        Message = message;
        IssueType = issueType;
    }

    public Error AsError() 
        => this as Error ?? Error.Of(new InvalidOperationException(Message));
    public static implicit operator string(Issue issue) => issue.Message;
    public string Message { get; }
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public IssueType IssueType { get; }

    #region Equality methods
    public bool Equals(Issue other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Message == other.Message && IssueType == other.IssueType;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Issue)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Message, (int)IssueType);

    public static bool operator ==(Issue left, Issue right) => Equals(left, right);

    public static bool operator !=(Issue left, Issue right) => !Equals(left, right);
    #endregion
}

public class Error: Issue
{
    public Exception Exception { get; }
    private Error(Exception exception) : base(exception.Message, IssueType.Error)
    {
        Exception = exception;
    }

    public static Error Of(Exception exception) => new(exception);
    public static implicit operator Error(Exception exception) => new(exception);
}

public class Warning : Issue
{
    private Warning(string message) : base(message, IssueType.Warning) { }

    public static Warning Of(string message) => new(message);
    public static implicit operator Warning(string message) => new(message);
}

public static class IssueExtensions
{
    public static string Join(this IEnumerable<Issue> issues) => issues.Select(x => x.Message).Join(", ");
}