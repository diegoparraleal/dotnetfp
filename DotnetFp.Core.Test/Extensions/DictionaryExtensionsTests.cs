using DotnetFp.Core.Extensions;
using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Extensions;
using static TestHelper;

public class DictionaryExtensionsTests
{
    [Test]
    public void Maybe_get_should_work()
    {
        // Assert
        Assert.That(Numbers.MaybeGet(1), Is.EqualTo(Maybe.Of("One")));
        Assert.That(Numbers.MaybeGet(2), Is.EqualTo(Maybe.Of("Two")));
        Assert.That(Numbers.MaybeGet(3), Is.EqualTo(Maybe.Nothing<string>()));
        Assert.That(Numbers.MaybeGetOrElse(3, "Tres"), Is.EqualTo("Tres"));
    }
}