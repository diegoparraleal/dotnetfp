using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DotnetFp.Core.Extensions;

namespace DotnetFp.Core.Utils;

public static class ArgumentValidation
{
    [return: NotNull] 
    public static T ThrowIfNull<T>(this T value, [CallerArgumentExpression("value")] string paramName = null!) 
        => value.WhenNullThen(() => throw new ArgumentException($"'{paramName}' cannot be empty."))!;
}