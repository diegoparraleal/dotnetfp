using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;

public class ExpectedTests
{
    [Test]
    public void Should_create_expected_objects()
    {
        // Assert
        Assert.That(Expected.Success(Something), Is.EqualTo(Success));
        Assert.That((Expected<string>) Something, Is.EqualTo(Success));
        Assert.That(Something.AsExpected(), Is.EqualTo(Success));
        Assert.That(Expected.Failure<string>(DefaultError), Is.EqualTo(Failure));
        Assert.That((Expected<string>) DefaultError!, Is.EqualTo(Failure));
        Assert.That(DefaultError.AsExpectedError<string>(), Is.EqualTo(Failure));
    }
    
    [Test]
    public void Should_generate_string_representations()
    {
        // Arrange
        Assert.That(Success.ToString(), Is.EqualTo($"({Something})"));
        Assert.That(Failure.ToString(), Is.EqualTo($"(Error: {DefaultError.Message})"));
    }
    
    [Test]
    public void Should_be_able_to_extract_values()
    {
        // Arrange
        Assert.That(Success.IsSuccessful, Is.True);
        Assert.That(Success.TryGet(out var item1), Is.True);
        Assert.That(Success.TryGetError(out var err1), Is.False);
        Assert.That(item1, Is.EqualTo(Something));
        Assert.That(err1, Is.Null);
        
        Assert.That(Failure.IsSuccessful, Is.False);
        Assert.That(Failure.TryGet(out var item2), Is.False);
        Assert.That(Failure.TryGetError(out var err2), Is.True);
        Assert.That(item2, Is.EqualTo(null));
        Assert.That(err2, Is.EqualTo(DefaultError));
    }
    
    [Test]
    public void Should_be_able_to_use_match_method()
    {
        // Assert
        Success.Match(
            success: x => AssertThatAndReturnTrue(x, Is.EqualTo(Something)),
            failure: _ => throw new Exception("Invalid"));
        Failure.Match(
            success: _ => throw new Exception("Invalid"),
            failure: err => AssertThatAndReturnTrue(err, Is.EqualTo(DefaultError)));
    }
    
    [Test]
    public void Should_be_able_to_use_or_methods()
    {
        // Assert 
        Assert.That(Success.OrFallback("xxx"), Is.EqualTo(Something));
        Assert.That(Success.OrFallback(() =>"xxx"), Is.EqualTo(Something));
        Assert.That(Success.OrRethrow(), Is.EqualTo(Something));
        Assert.That(Failure.OrFallback("xxx"), Is.EqualTo("xxx"));
        Assert.That(Failure.OrFallback(() => "xxx"), Is.EqualTo("xxx"));
        Assert.Throws<Exception>(() => Failure.OrRethrow());
    }
    
    [Test]
    public void Should_be_able_to_use_monad_methods()
    {
        // Assert
        Assert.That(Success.Map(ToUpper).OrRethrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That(Success.Bind(ExpectedToUpper).OrRethrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That(Failure.Map(ToUpper).OrFallback(() => null!), Is.EqualTo(null));
        Assert.That(Failure.Bind(ExpectedToUpper).OrFallback(() => null!), Is.EqualTo(null));
    }
    
    [Test]
    public async Task Should_be_able_to_handle_async_lambdas()
    {
        // Assert
        Assert.That((await Success.MapAsync(ToUpperAsync)).OrRethrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That((await Success.BindAsync(ExpectedToUpperAsync)).OrRethrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That(await Maybe<string>.Nothing.OrElseAsync(GenerateDefaultAsync), Is.EqualTo(Default));
    }
    
    [Test]
    public void Should_be_able_to_deconstruct()
    {
        // Act
        var (isSuccess1, value1, error1) = Success;
        var (isSuccess2, value2, error2) = Failure;
        
        // Assert
        Assert.That(isSuccess1, Is.True);
        Assert.That(value1, Is.EqualTo(Something));
        Assert.That(error1, Is.Null);
        
        Assert.That(isSuccess2, Is.False);
        Assert.That(value2, Is.EqualTo(null));
        Assert.That(error2, Is.EqualTo(DefaultError));

    }

    [Test]
    public async Task Should_wrap_unsafe_code_automatically_into_expected()
    {
        // Assert working cases
        Assert.That(Try.Run(() => ToUpper(Something)).IsSuccessful, Is.True);
        Assert.That(Try.Run(() => ToUpper(Something)).OrRethrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That((await Try.RunAsync(() => ToUpperAsync(Something))).IsSuccessful, Is.True);
        Assert.That((await Try.RunAsync(() => ToUpperAsync(Something))).OrRethrow(), Is.EqualTo(Something.ToUpper()));
        
        // Assert not working cases
        Assert.That(Try.Run(() => ToUpperUnsafe(Something)).IsSuccessful, Is.False);
        Assert.That(Try.Run(() => ToUpperUnsafe(Something)).ErrorOrThrow().Message, Is.EqualTo(DefaultError.Message));
        Assert.That((await Try.RunAsync(() => ToUpperUnsafeAsync(Something))).IsSuccessful, Is.False);
        Assert.That((await Try.RunAsync(() => ToUpperUnsafeAsync(Something))).ErrorOrThrow().Message, Is.EqualTo(DefaultError.Message));
    }
}