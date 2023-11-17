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
        
    [Test]
    public void Collection_can_create_partitions()
    {
        // Act
        var (even, odd) = SequenceOneToTen.Partition(x => x % 2 == 0);
        
        // Assert
        Assert.That(even, Is.EqualTo(new [] { 2, 4, 6, 8, 10}));
        Assert.That(odd, Is.EqualTo(new [] { 1, 3, 5, 7, 9}));
    }
    
    [Test]
    public void Collection_of_string_can_have_join()
    {
        // Assert
        Assert.That(NumberTexts.Join(','), Is.EqualTo("One,Two"));
        Assert.That(NumberTexts.Join(", "), Is.EqualTo("One, Two"));
    }
    
    [Test]
    public void Collection_can_be_transformed_to_readonly()
    {
        // Act
        var list = new List<int> { 1, 2, 3 };
        
        // Assert
        Assert.That(OneTwoSequence.AsReadOnly(), Is.EquivalentTo(OneTwoSequence));
        Assert.That(list.AsReadOnly(), Is.EqualTo(list));
    }
    
    [Test]
    public void Can_convert_null_to_collection()
    {
        // Arrange
        List<string> nullColl = null!; 
        
        // Assert
        Assert.That(nullColl.ToEmptyIfNull(), Is.EquivalentTo(Array.Empty<string>()));
    }
}