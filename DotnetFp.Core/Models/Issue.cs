using DotnetFp.Core.Extensions;

namespace DotnetFp.Core.Models;

public enum IssueType
{
    Error,
    Warning
};

public abstract record Issue(string Message, IssueType IssueType)
{
    public static implicit operator string(Issue issue) => issue.Message;
}

public record Error: Issue
{
    public Exception Exception { get; init; }
    private Error(Exception exception) : base(exception.Message, IssueType.Error)
    {
        Exception = exception;
    }

    public static Error Of(Exception exception) => new(exception);
    public static implicit operator Error(Exception exception) => new(exception);
}

public record Warning(string Message): Issue(Message, IssueType.Warning)
{
    public static Warning Of(string message) => new(message);
    public static implicit operator Warning(string message) => new(message);
}

public static class IssueExtensions
{
    public static string Join(this IEnumerable<Issue> issues) => issues.Select(x => x.Message).Join(", ");
}