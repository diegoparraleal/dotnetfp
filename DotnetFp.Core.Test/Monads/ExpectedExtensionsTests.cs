using DotnetFp.Core.Extensions;
using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;

public class ExpectedExtensionsTests
{
    [Test]
    public void Expected_AsEnumerable_works()
    {
        // Act
        var sut = Expected.Success(Something);
        
        // Assert
        Assert.That(sut.AsEnumerable(), Is.EquivalentTo(Something.Once()));
    }
    
    [Test]
    public void Expected_extensions_for_collections_work()
    {
        // Act
        var sut = new [] { Expected.Success(Something), Expected.Failure<string>(DefaultError) };
        
        // Assert
        Assert.That(sut.SelectValidValues(), Is.EquivalentTo(Something.Once()));
        Assert.That(sut.SelectErrors(), Is.EquivalentTo(DefaultError.Once()));
        Assert.That(sut.WhereSuccessful().Count(), Is.EqualTo(1));
        Assert.That(sut.WhereFailure().Count(), Is.EqualTo(1));

        var (success, failures) = sut.Partition();
        Assert.That(success, Is.EquivalentTo(Something.Once()));
        Assert.That(failures, Is.EquivalentTo(DefaultError.Once()));
    }

    [Test]
    public void Expected_extensions_for_maybe_work()
    {
        // Assert
        Assert.That(Expected.Success(Something).AsMaybe(), Is.EqualTo(Maybe.Of(Something)));
        Assert.That(Expected.Failure<string>(DefaultError).AsMaybe(), Is.EqualTo(Maybe.Nothing<string>()));
    }
}