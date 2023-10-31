namespace DotnetFp.Core.Test;

public class MaybeTests
{
    [Test]
    public void Should_create_maybe_out_of_something()
    {
        // Arrange
        const string something = "something";
        
        // Act
        var sut = Maybe.Of(something);
        
        // Assert
        Assert.That(sut.ToString(), Is.EqualTo($"({something})"));
        Assert.That(sut.HasValue, Is.True);
        Assert.That(sut.TryGet(out var value), Is.True);
        Assert.That(value, Is.EqualTo(something));
        Assert.That(sut, Is.EqualTo(something.AsMaybe()));
        Assert.That(sut.OrElse("xxx"), Is.EqualTo(something));
        Assert.That(sut.OrThrow(), Is.EqualTo(something));
        Assert.That(sut.OrNull(), Is.EqualTo(something));
        Assert.That(sut.Map(x => x.ToUpper()).OrNull(), Is.EqualTo(something.ToUpper()));
        Assert.That(sut.Bind(x => Maybe.Of(x.ToUpper())).OrNull(), Is.EqualTo(something.ToUpper()));
    }
    
    [Test]
    public void Should_create_maybe_out_of_nothing()
    {
        // Act
        var sut = Maybe<string>.Nothing;
        
        // Assert
        Assert.That(sut.ToString(), Is.EqualTo("(Nothing)"));
        Assert.That(sut.HasValue, Is.False);
        Assert.That(sut.TryGet(out var value), Is.False);
        Assert.That(value, Is.EqualTo(null));
        Assert.That(sut, Is.EqualTo(Maybe<string>.Nothing));
        Assert.That(sut.OrElse("xxx"), Is.EqualTo("xxx"));
        Assert.Throws<InvalidOperationException>(() => sut.OrThrow());
        Assert.That(sut.OrNull(), Is.EqualTo(null));
        Assert.That(sut.Map(x => x.ToUpper()).OrNull(), Is.EqualTo(null));
        Assert.That(sut.Bind(x => Maybe.Of(x.ToUpper())).OrNull(), Is.EqualTo(null));
    }
}