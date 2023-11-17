using DotnetFp.Core.Monads;
using NUnit.Framework.Constraints;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;

public class ChoiceTests
{
    [Test]
    public void Should_create_choice_object()
    {
        // Assert
        Assert.That((Choice<string, int>)Something, Is.EqualTo(ChoiceStr));
        Assert.That((Choice<string, int>)Number, Is.EqualTo(ChoiceInt));
        Assert.That(Something.AsChoice<string, int>(), Is.EqualTo(ChoiceStr));
        Assert.That(Number.AsChoice<string, int>(), Is.EqualTo(ChoiceInt));
    }

    [Test]
    public void Should_be_able_to_use_ToString_properly()
    {
        // Assert
        Assert.That(ChoiceStr.ToString(), Is.EqualTo($"({Something})"));
        Assert.That(ChoiceInt.ToString(), Is.EqualTo($"({Number})"));
    }

    [Test]
    public void Should_be_able_to_use_verify_left_and_right_existence()
    {
        // Assert
        Assert.That(ChoiceStr.IsLeft, Is.True);
        Assert.That(ChoiceStr.IsRight, Is.False);
        Assert.That(ChoiceInt.IsLeft, Is.False);
        Assert.That(ChoiceInt.IsRight, Is.True);
        Assert.That(ChoiceStr.Is<string>, Is.True);
        Assert.That(ChoiceStr.Is<int>, Is.False);
        Assert.That(ChoiceInt.Is<string>, Is.False);
        Assert.That(ChoiceInt.Is<int>, Is.True);
    }

    [Test]
    public void Should_be_able_to_use_try_get_method()
    {
        // Assert
        Assert.That(ChoiceStr.TryGet(out string str1), Is.True);
        Assert.That(str1, Is.EqualTo(Something));
        Assert.That(ChoiceStr.TryGet(out int number1), Is.False);
        Assert.That(number1, Is.EqualTo(0));
        Assert.That(ChoiceInt.TryGet(out string str2), Is.False);
        Assert.That(str2, Is.EqualTo(default));
        Assert.That(ChoiceInt.TryGet(out int number2), Is.True);
        Assert.That(number2, Is.EqualTo(Number));
    }
    
    [Test]
    public void Should_be_able_to_use_match_method()
    {
        // Assert
        ChoiceStr.Match(
            left: x => AssertThatAndReturnTrue(x, Is.EqualTo(Something)),
            right: x => AssertThatAndReturnTrue(x, Is.Null));
        ChoiceInt.Match(
            left: x => AssertThatAndReturnTrue(x, Is.Null),
            right: x => AssertThatAndReturnTrue(x, Is.EqualTo(Number)));
    }

    [Test]
    public void Should_be_able_to_use_left_right_or_throw_methods()
    {
        // Assert
        Assert.That(ChoiceStr.LeftOrThrow(), Is.EqualTo(Something));
        Assert.Throws<InvalidOperationException>(() => ChoiceStr.RightOrThrow());
        Assert.That(ChoiceStr.ValueOrThrow<string>(), Is.EqualTo(Something));
        Assert.Throws<InvalidOperationException>(() => ChoiceInt.LeftOrThrow());
        Assert.That(ChoiceInt.RightOrThrow(), Is.EqualTo(Number));
        Assert.That(ChoiceInt.ValueOrThrow<int>(), Is.EqualTo(Number));
    }

    [Test]
    public void Should_be_able_to_use_basic_monad_methods()
    {
        // Assert
        Assert.That(ChoiceStr.Map(ToUpper, MultiplyByTwo).LeftOrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That(ChoiceInt.Map(ToUpper, MultiplyByTwo).RightOrThrow(), Is.EqualTo(Number * 2));
        
        Assert.That(ChoiceStr.MapLeft(ToUpper).LeftOrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That(ChoiceInt.MapRight(MultiplyByTwo).RightOrThrow(), Is.EqualTo(Number * 2));

        Assert.That(ChoiceStr.Bind(ChoiceToUpper, ChoiceMultiplyByTwo).LeftOrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That(ChoiceInt.Bind(ChoiceToUpper, ChoiceMultiplyByTwo).RightOrThrow(), Is.EqualTo(Number * 2));
        
        Assert.That(ChoiceStr.BindLeft(ChoiceToUpper).LeftOrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That(ChoiceInt.BindRight(ChoiceMultiplyByTwo).RightOrThrow(), Is.EqualTo(Number * 2));
    }
    
    
    [Test]
    public async Task Should_be_able_to_handle_async_lambdas()
    {
        // Assert
        Assert.That((await ChoiceStr.MapAsync(ToUpperAsync, MultiplyByTwoAsync)).LeftOrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That((await ChoiceInt.MapAsync(ToUpperAsync, MultiplyByTwoAsync)).RightOrThrow(), Is.EqualTo(Number * 2));
        
        Assert.That((await ChoiceStr.MapLeftAsync(ToUpperAsync)).LeftOrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That((await ChoiceInt.MapRightAsync(MultiplyByTwoAsync)).RightOrThrow(), Is.EqualTo(Number * 2));
        
        Assert.That((await ChoiceStr.BindAsync(ChoiceToUpperAsync, ChoiceMultiplyByTwoAsync)).LeftOrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That((await ChoiceInt.BindAsync(ChoiceToUpperAsync, ChoiceMultiplyByTwoAsync)).RightOrThrow(), Is.EqualTo(Number * 2));
        
        Assert.That((await ChoiceStr.BindLeftAsync(ChoiceToUpperAsync)).LeftOrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That((await ChoiceInt.BindRightAsync(ChoiceMultiplyByTwoAsync)).RightOrThrow(), Is.EqualTo(Number * 2));
    }

    [Test]
    public void Should_be_able_to_use_deconstruct_methods()
    {
        // Act
        var (isLeft1, isRight1, left1, right1) = ChoiceStr;
        var (isLeft2, isRight2, left2, right2) = ChoiceInt;

        // Assert
        Assert.That(isLeft1, Is.True);
        Assert.That(isRight1, Is.False);
        Assert.That(left1, Is.EqualTo(Something));
        Assert.That(right1, Is.EqualTo(0));
        
        Assert.That(isLeft2, Is.False);
        Assert.That(isRight2, Is.True);
        Assert.That(left2, Is.EqualTo(default));
        Assert.That(right2, Is.EqualTo(Number));
    }
}