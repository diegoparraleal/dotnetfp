namespace DotnetFp.Core.Test;

public class ObjectExtensionsTests
{
    [Test]
    public void ApplyIf_should_work()
    {
        // Arrange
        const int  sut = 1;
        int GeneratorFn() => 2;

        // Assert
        Assert.That(sut.ApplyIf(true, GeneratorFn), Is.EqualTo(2));
        Assert.That(sut.ApplyIf(false, GeneratorFn), Is.EqualTo(sut));
    }

    [Test]
    public void WhenNullThen_works()
    {
        // Arrange
        const string bye = "Bye!";
        const string notNullStr = "Hi!";
        string nullStr = null!;
        
        // Assert
        Assert.That(nullStr.WhenNullThen(bye), Is.EqualTo(bye));
        Assert.That(nullStr.WhenNullThen(() => bye), Is.EqualTo(bye));
        Assert.That(notNullStr.WhenNullThen(bye), Is.EqualTo(notNullStr));
        Assert.That(notNullStr.WhenNullThen(() => bye), Is.EqualTo(notNullStr));
    }
}