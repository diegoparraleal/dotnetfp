namespace DotnetFp.Core.Test;
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