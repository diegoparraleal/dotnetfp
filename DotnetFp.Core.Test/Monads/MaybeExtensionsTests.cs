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
}