using DotnetFp.Core.Extensions;

namespace DotnetFp.Core.Test.Extensions;
using static TestHelper;
public class TupleExtensionsTests
{
    [Test]
    public void AsEnumerable_should_work()
    {
        // Assert
        Assert.That((1, 2).AsEnumerable(), Is.EquivalentTo(new [] { 1, 2 }));
        Assert.That((1, 2, 3).AsEnumerable(), Is.EquivalentTo(new [] { 1, 2, 3 }));
        Assert.That((1, 2, 3, 4).AsEnumerable(), Is.EquivalentTo(new [] { 1, 2, 3, 4 }));
        Assert.That((1, 2, 3, 4, 5).AsEnumerable(), Is.EquivalentTo(new [] { 1, 2, 3, 4, 5 }));
    }
    
    #region GetAwaiter tests
    [Test]
    public async Task GetAwaiter_with_2_elements_should_work()
    {
        // Act
        var (a, b) = await (MultiplyByTwoAsync(1), MultiplyByTwoAsync(2)); 

        // Assert
        Assert.That(a, Is.EqualTo(2));
        Assert.That(b, Is.EqualTo(4));
    }
    
    [Test]
    public async Task GetAwaiter_with_3_elements_should_work()
    {
        // Act
        var (a, b, c) = await (MultiplyByTwoAsync(1), MultiplyByTwoAsync(2), MultiplyByTwoAsync(3)); 

        // Assert
        Assert.That(a, Is.EqualTo(2));
        Assert.That(b, Is.EqualTo(4));
        Assert.That(c, Is.EqualTo(6));
    }
    
    [Test]
    public async Task GetAwaiter_with_4_elements_should_work()
    {
        // Act
        var (a, b, c, d) = await (MultiplyByTwoAsync(1), MultiplyByTwoAsync(2), MultiplyByTwoAsync(3), MultiplyByTwoAsync(4)); 

        // Assert
        Assert.That(a, Is.EqualTo(2));
        Assert.That(b, Is.EqualTo(4));
        Assert.That(c, Is.EqualTo(6));
        Assert.That(d, Is.EqualTo(8));
    }
    
    [Test]
    public async Task GetAwaiter_with_5_elements_should_work()
    {
        // Act
        var (a, b, c, d, e) = await (MultiplyByTwoAsync(1), MultiplyByTwoAsync(2), MultiplyByTwoAsync(3), MultiplyByTwoAsync(4), MultiplyByTwoAsync(5)); 

        // Assert
        Assert.That(a, Is.EqualTo(2));
        Assert.That(b, Is.EqualTo(4));
        Assert.That(c, Is.EqualTo(6));
        Assert.That(d, Is.EqualTo(8));
        Assert.That(e, Is.EqualTo(10));
    }
    #endregion
}