using DotnetFp.Core.Extensions;

namespace DotnetFp.Core.Test.Extensions;
using static TestHelper;

public class EnumerableExtensionsTests
{
    [Test]
    public async Task Select_should_work_for_task_of_collection()
    {
        // Assert
        Assert.That(await GenerateSequenceAsync().Select(MultiplyByTwo), Is.EquivalentTo(SequenceTimesTwo));
    }
    
    [Test]
    public async Task SelectAsync_should_work_for_async_transformer()
    {
        // Assert
        Assert.That(await Sequence.SelectAsync(MultiplyByTwoAsync), Is.EquivalentTo(SequenceTimesTwo));
    }
    
    [Test]
    public async Task SelectMany_should_work_for_task_of_collection()
    {
        // Assert
        Assert.That(await GenerateOneTwoSequenceAsync().SelectMany(NextSequence), Is.EquivalentTo(SequenceOneToTen));
    }
    
    [Test]
    public async Task SelectManyAsync_should_work_for_async_transformer()
    {
        // Assert
        Assert.That(await OneTwoSequence.SelectManyAsync(NextSequenceAsync), Is.EquivalentTo(SequenceOneToTen));
    }
    
    [Test]
    public void Project_should_work()
    {
        // Assert
        Assert.That(Sequence.Project(MultiplyByTwo), Is.EquivalentTo(new [] { (1,2), (2,4), (3,6), (4,8), (5,10) }));
    }
    
    [Test]
    public async Task ProjectAsync_should_work()
    {
        // Assert
        Assert.That(await Sequence.ProjectAsync(MultiplyByTwoAsync), Is.EquivalentTo(new [] { (1,2), (2,4), (3,6), (4,8), (5,10) }));
    }
    
    [Test]
    public async Task Where_should_work_for_task_of_collection()
    {
        // Assert
        Assert.That(await GenerateSequenceAsync().Where(IsEven), Is.EquivalentTo(new [] { 2, 4 }));
    }
    
    [Test]
    public async Task WhereAsync_should_work_for_async_transformer()
    {
        // Assert
        Assert.That(await Sequence.WhereAsync(IsEvenAsync), Is.EquivalentTo(new [] { 2, 4 }));
    }
}