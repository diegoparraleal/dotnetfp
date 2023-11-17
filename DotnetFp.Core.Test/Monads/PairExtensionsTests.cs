using DotnetFp.Core.Extensions;
using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;

public class PairExtensionsTests
{
    [Test]
    public void Pair_AsEnumerable_works()
    {
        // Assert
        Assert.That(PairInt.AsEnumerable(), Is.EquivalentTo(OneTwoSequence));
    }
    
    [Test]
    public void Pair_SelectMany_works()
    {
        // Arrange
        var sut = Pair.Of(new[] { 1, 2 }.AsEnumerable(), new[] { 3, 4 });
        
        // Assert
        Assert.That(sut.SelectMany(), Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
    }
}