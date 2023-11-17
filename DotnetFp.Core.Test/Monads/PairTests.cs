using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;

public class PairTests
{
    [Test]
    public void Should_create_pair_object()
    {
        // Assert
        Assert.That(Pair.Of(1, 2), Is.EqualTo(PairInt));
        Assert.That((Pair<int>) (1, 2), Is.EqualTo(PairInt));
        Assert.That((1, 2).AsPair(), Is.EqualTo(PairInt));
    }

    [Test]
    public void Should_be_able_to_use_ToString_properly()
    {
        // Assert
        Assert.That(PairInt.ToString(), Is.EqualTo($"(1, 2)"));
    }

    [Test]
    public void Should_be_able_to_use_basic_monad_methods()
    {
        // Assert
        Assert.That(PairInt.Map(MultiplyByTwo).AsTuple(), Is.EqualTo((2, 4)));
        Assert.That(PairInt.Bind(PairMultiplyByTwo).AsTuple(), Is.EqualTo((2, 4)));
    }

    [Test]
    public void Should_be_able_to_use_deconstruct_methods()
    {
        // Act
        var (first, second) = PairInt;

        // Assert
        Assert.That(first, Is.EqualTo(1));
        Assert.That(second, Is.EqualTo(2));
    }  
    
    [Test]
    public async Task Should_be_able_to_handle_async_lambdas()
    {
        // Assert
        Assert.That((await PairInt.MapAsync(MultiplyByTwoAsync)).AsTuple(), Is.EqualTo((2, 4)));
        Assert.That((await PairInt.BindAsync(PairMultiplyByTwoAsync)).AsTuple(), Is.EqualTo((2, 4)));
    }
}