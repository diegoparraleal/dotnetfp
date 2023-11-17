using DotnetFp.Core.Extensions;
using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;

public class ValidatedExtensionsTests
{
    [Test]
    public void Validated_AsEnumerable_works()
    {
        // Act
        var sut = Validated.Valid(Something);
        
        // Assert
        Assert.That(sut.AsEnumerable(), Is.EquivalentTo(Something.Once()));
    }
    
    [Test]
    public void Validated_extensions_for_collections_work()
    {
        // Act
        var sut = new [] { Validated.Valid(Something), Validated.Invalid<string>(DefaultIssue) };
        
        // Assert
        Assert.That(sut.SelectValidValues(), Is.EquivalentTo(Something.Once()));
        Assert.That(sut.SelectErrors(), Is.EquivalentTo(DefaultIssue.Once()));
        Assert.That(sut.WhereValid().Count(), Is.EqualTo(1));
        Assert.That(sut.WhereInvalid().Count(), Is.EqualTo(1));

        var (valid, invalid) = sut.Partition();
        Assert.That(valid, Is.EquivalentTo(Something.Once()));
        Assert.That(invalid, Is.EquivalentTo(DefaultIssue.Once()));
    }

    [Test]
    public void Validated_extensions_for_maybe_work()
    {
        // Assert
        Assert.That(Validated.Valid(Something).AsMaybe(), Is.EqualTo(Maybe.Of(Something)));
        Assert.That(Validated.Invalid<string>(DefaultIssue).AsMaybe(), Is.EqualTo(Maybe.Nothing<string>()));
    }
    
    [Test]
    public void Validated_extensions_for_expected_work()
    {
        // Assert
        Assert.That(Validated.Valid(Something).AsExpected(), Is.EqualTo(Expected.Success(Something)));
        Assert.That(Validated.Invalid<string>(DefaultIssue).AsExpected(), Is.EqualTo(Expected.Failure<string>(DefaultIssue.AsError())));
    }
}