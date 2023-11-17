using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;
public class MaybeTests
{
    [Test]
    public void Should_create_maybe_objects()
    {
        // Arrange
        Assert.That(Maybe.Of(Something), Is.EqualTo(MaybeStr));
        Assert.That((Maybe<string>) Something, Is.EqualTo(MaybeStr));
        Assert.That(Something.AsMaybe(), Is.EqualTo(MaybeStr));
        Assert.That((Maybe<string>) NullStr!, Is.EqualTo(Nothing));
        Assert.That(NullStr.AsMaybe(), Is.EqualTo(Nothing));
    }

    [Test]
    public void Should_create_string_representation()
    {
        // Assert
        Assert.That(MaybeStr.ToString(), Is.EqualTo($"({Something})"));
        Assert.That(Nothing.ToString(), Is.EqualTo("(Nothing)"));
    }

    [Test]
    public void Should_be_able_to_extract_maybe_values()
    {
        // Assert
        Assert.That(MaybeStr.HasValue, Is.True);
        Assert.That(MaybeStr.TryGet(out var item1), Is.True);
        Assert.That(item1, Is.EqualTo(Something));
        Assert.That(MaybeStr, Is.EqualTo(Something.AsMaybe()));
        Assert.That(Nothing.HasValue, Is.False);
        Assert.That(Nothing.TryGet(out var item2), Is.False);
        Assert.That(item2, Is.EqualTo(null));
        Assert.That(Nothing, Is.EqualTo(Maybe<string>.Nothing));
    }
    
    [Test]
    public void Should_be_able_to_use_match_method()
    {
        // Assert
        MaybeStr.Match(
            some: x => AssertThatAndReturnTrue(x, Is.EqualTo(Something)),
            none: () => throw new Exception("Invalid"));
        Nothing.Match(
            some: _ => throw new Exception("Invalid"),
            none: () => AssertThatAndReturnTrue(true, Is.True));
    }

    [Test]
    public void Should_be_able_to_use_or_functions()
    {
        // Assert
        Assert.That(MaybeStr.OrElse("xxx"), Is.EqualTo(Something));
        Assert.That(MaybeStr.OrElse(() =>"xxx"), Is.EqualTo(Something));
        Assert.That(MaybeStr.OrThrow(), Is.EqualTo(Something));
        Assert.That(MaybeStr.OrNull(), Is.EqualTo(Something));
        Assert.That(Nothing.OrElse("xxx"), Is.EqualTo("xxx"));
        Assert.That(Nothing.OrElse(() => "xxx"), Is.EqualTo("xxx"));
        Assert.Throws<InvalidOperationException>(() => Nothing.OrThrow());
        Assert.That(Nothing.OrNull(), Is.EqualTo(null));
    }

    [Test]
    public void Should_be_able_to_use_monad_functions()
    {
        // Assert
        Assert.That(MaybeStr.Map(ToUpper).OrNull(), Is.EqualTo(Something.ToUpper()));
        Assert.That(MaybeStr.Bind(MaybeToUpper).OrNull(), Is.EqualTo(Something.ToUpper()));
        Assert.That(Nothing.Map(ToUpper).OrNull(), Is.EqualTo(null));
        Assert.That(Nothing.Bind(MaybeToUpper).OrNull(), Is.EqualTo(null));
    }

    [Test]
    public async Task Should_be_able_to_handle_async_lambdas()
    {
        // Act
        var sut = Maybe.Of(Something);
        
        // Assert
        Assert.That((await sut.MapAsync(ToUpperAsync)).OrNull(), Is.EqualTo(Something.ToUpper()));
        Assert.That((await sut.BindAsync(MaybeToUpperAsync)).OrNull(), Is.EqualTo(Something.ToUpper()));
        Assert.That(await Maybe<string>.Nothing.OrElseAsync(GenerateDefaultAsync), Is.EqualTo(Default));
    }
    
    [Test]
    public void Should_be_able_to_deconstruct()
    {
        // Arrange
        var (hasValue1, value1) = MaybeStr;
        Assert.That(hasValue1, Is.True);
        Assert.That(value1, Is.EqualTo(Something));
        var (hasValue2, value2) = Nothing;
        Assert.That(hasValue2, Is.False);
        Assert.That(value2, Is.EqualTo(null));
    }
}