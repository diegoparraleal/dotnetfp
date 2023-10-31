using DotnetFp.Core.Utils;

namespace DotnetFp.Core.Test.Utils;

public class ArgumentValidationTests
{
    [Test]
    public void ThrowIfNull_should_work()
    {
        // Arrange
        string nullStr = null!;
        const string notNullStr = "Hi!";
        
        // Assert
        Assert.That(notNullStr.ThrowIfNull(), Is.EqualTo(notNullStr));
        Assert.Throws<ArgumentException>(() => nullStr.ThrowIfNull());
        Assert.Throws<ArgumentException>(() => nullStr.ThrowIfNull("Error"), "Error");
    }
}