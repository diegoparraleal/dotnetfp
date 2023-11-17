using DotnetFp.Core.Extensions;
using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;

public class MaybeExtensionsTests
{
    [Test]
    public void Maybe_AsEnumerable_works()
    {
        // Act
        var sut = Maybe.Of(Something);
        
        // Assert
        Assert.That(sut.AsEnumerable(), Is.EquivalentTo(Something.Once()));
    }
    
    [Test]
    public void Maybe_extensions_for_collections_work()
    {
        // Act
        var sut = new [] { Maybe.Of(Something), Maybe.Nothing<string>() };
        
        // Assert
        Assert.That(sut.SelectValues(), Is.EquivalentTo(Something.Once()));
        Assert.That(sut.WhereNothing().Count(), Is.EqualTo(1));
        Assert.That(sut.WhereHasValue().Count(), Is.EqualTo(1));
    }
}