namespace DotnetFp.Core.Test;
using static TestHelper;
public class MaybeTests
{
    [Test]
    public void Should_create_maybe_out_of_something()
    {
        // Act
        var sut = Maybe.Of(Something);
        
        // Assert
        Assert.That(sut.ToString(), Is.EqualTo($"({Something})"));
        Assert.That(sut.HasValue, Is.True);
        Assert.That(sut.TryGet(out var item), Is.True);
        Assert.That(item, Is.EqualTo(Something));
        Assert.That(sut, Is.EqualTo(Something.AsMaybe()));
        Assert.That(sut.OrElse("xxx"), Is.EqualTo(Something));
        Assert.That(sut.OrThrow(), Is.EqualTo(Something));
        Assert.That(sut.OrNull(), Is.EqualTo(Something));
        Assert.That(sut.Map(ToUpper).OrNull(), Is.EqualTo(Something.ToUpper()));
        Assert.That(sut.Bind(MaybeToUpper).OrNull(), Is.EqualTo(Something.ToUpper()));
        
        var (hasValue, value) = sut;
        Assert.That(hasValue, Is.True);
        Assert.That(value, Is.EqualTo(Something));
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
        
        var (hasValue, value) = sut;
        Assert.That(hasValue, Is.True);
        Assert.That(value, Is.EqualTo(Something));
    }
    
    [Test]
    public void Should_create_maybe_out_of_nothing()
    {
        // Act
        var sut = Maybe<string>.Nothing;
        
        // Assert
        Assert.That(sut.ToString(), Is.EqualTo("(Nothing)"));
        Assert.That(sut.HasValue, Is.False);
        Assert.That(sut.TryGet(out var item), Is.False);
        Assert.That(item, Is.EqualTo(null));
        Assert.That(sut, Is.EqualTo(Maybe<string>.Nothing));
        Assert.That(sut.OrElse("xxx"), Is.EqualTo("xxx"));
        Assert.Throws<InvalidOperationException>(() => sut.OrThrow());
        Assert.That(sut.OrNull(), Is.EqualTo(null));
        Assert.That(sut.Map(ToUpper).OrNull(), Is.EqualTo(null));
        Assert.That(sut.Bind(MaybeToUpper).OrNull(), Is.EqualTo(null));
        
        var (hasValue, value) = sut;
        Assert.That(hasValue, Is.False);
        Assert.That(value, Is.EqualTo(null));
    }
}