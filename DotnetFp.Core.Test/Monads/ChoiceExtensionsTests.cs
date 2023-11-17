using DotnetFp.Core.Extensions;
using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;

public class ChoiceExtensionsTests
{
    [Test]
    public void Choice_AsEnumerable_works()
    {
        // Assert
        Assert.That(ChoiceStr.LeftAsEnumerable(), Is.EquivalentTo(Something.Once()));
        Assert.That(ChoiceInt.RightAsEnumerable(), Is.EquivalentTo(Number.Once()));
    }
    
    [Test]
    public void Choice_extensions_for_collections_work()
    {
        // Arrange
        var sut = new [] { ChoiceStr, ChoiceInt };
        
        // Act
        var (left, right) = sut.Partition();
        
        // Assert
        Assert.That(sut.SelectLeftValues(), Is.EquivalentTo(Something.Once()));
        Assert.That(sut.SelectRightValues(), Is.EquivalentTo(Number.Once()));
        Assert.That(sut.WhereLeft().Count(), Is.EqualTo(1));
        Assert.That(sut.WhereRight().Count(), Is.EqualTo(1));
        Assert.That(left, Is.EquivalentTo(Something.Once()));
        Assert.That(right, Is.EquivalentTo(Number.Once()));
    }
}