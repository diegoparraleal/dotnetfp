using DotnetFp.Core.Extensions;
using DotnetFp.Core.Models;
using DotnetFp.Core.Monads;

namespace DotnetFp.Core.Test.Monads;
using static TestHelper;

public class ValidatedTests
{
    [Test]
    public void Should_create_validated_objects()
    {
        // Arrange
        var exception = new Exception(DefaultError.Message);
        
        // Assert
        Assert.That(Validated.Valid(Something), Is.EqualTo(Valid));
        Assert.That((Validated<string>) Something, Is.EqualTo(Valid));
        Assert.That(Something.AsValidated(), Is.EqualTo(Valid));
        
        Assert.That(Validated.Invalid<string>(DefaultIssue), Is.EqualTo(Invalid));
        Assert.That(Validated.Invalid<string>(DefaultIssue.Once().AsReadOnly()), Is.EqualTo(Invalid));
        Assert.That((Validated<string>) DefaultIssue!, Is.EqualTo(Invalid));
        Assert.That(DefaultIssue.AsValidationIssue<string>(), Is.EqualTo(Invalid));
        Assert.That(DefaultIssue.Once().AsReadOnly().AsValidationIssues<string>(), Is.EqualTo(Invalid));
        
        Assert.That(
            Validated.FromException<string>(exception), 
            Is.EqualTo(Validated.Invalid<string>((Error)exception)));
    }

    [Test]
    public void Should_validate_equality()
    {
        // Assert
        Assert.That(Validated.Valid(Something), Is.EqualTo(Valid));
        Assert.That(Validated.Valid(Something).GetHashCode(), Is.EqualTo(Valid.GetHashCode()));
        Assert.That(Validated.Valid(Something).Equals(Valid), Is.True);
        Assert.That(Validated.Valid(Something) == Valid, Is.True);
        Assert.That(Validated.Valid(Something) != Valid, Is.False);
    }
    
    [Test]
    public void Should_be_able_to_add_many_issues()
    {
        // Arrange
        var validated = Validated.Valid(Something);
        
        // Act
        validated = validated.AddIssue(DefaultIssue);
        validated = validated.AddIssue(DefaultError);
        
        // Assert
        Assert.That(validated.IsValid, Is.False);
        Assert.That(validated.IssuesOrThrow().Count(), Is.EqualTo(2));
        Assert.That(validated.ToString(), Is.EqualTo($"(Issues: {DefaultIssue.Message}, {DefaultError.Message})"));
        Assert.That(validated, Is.EqualTo(Validated.Invalid<string>(new List<Issue> { DefaultIssue, DefaultError })));
        Assert.That(validated, Is.EqualTo(Validated.Invalid<string>(DefaultIssue, DefaultError)));
    }

    [Test]
    public void Should_generate_string_representations()
    {
        // Assert
        Assert.That(Valid.ToString(), Is.EqualTo($"({Something})"));
        Assert.That(Invalid.ToString(), Is.EqualTo($"(Issues: {DefaultIssue.Message})"));
    }
    
    [Test]
    public void Should_be_able_to_extract_values()
    {
        // Assert
        Assert.That(Valid.IsValid, Is.True);
        Assert.That(Valid.TryGet(out var item1), Is.True);
        Assert.That(Valid.TryGetIssues(out var problems1), Is.False);
        Assert.That(item1, Is.EqualTo(Something));
        Assert.That(problems1, Is.Null);

        Assert.That(Invalid.IsValid, Is.False);
        Assert.That(Invalid.TryGet(out var item2), Is.False);
        Assert.That(Invalid.TryGetIssues(out var problems2), Is.True);
        Assert.That(item2, Is.EqualTo(null));
        Assert.That(problems2, Is.EquivalentTo(DefaultIssue.Once()));
    }
    
    [Test]
    public void Should_be_able_to_use_match_method()
    {
        // Assert
        Valid.Match(
            valid: x => AssertThatAndReturnTrue(x, Is.EqualTo(Something)),
            invalid: _ => throw new Exception("Invalid"));
        Invalid.Match(
            valid: _ => throw new Exception("Invalid"),
            invalid: issues => AssertThatAndReturnTrue(issues, Is.EquivalentTo(DefaultIssue.Once())));
    }
    
    [Test]
    public void Should_be_able_to_use_or_methods()
    {
        // Assert
        Assert.That(Valid.OrFallback("xxx"), Is.EqualTo(Something));
        Assert.That(Valid.OrFallback(() =>"xxx"), Is.EqualTo(Something));
        Assert.That(Valid.OrThrow(), Is.EqualTo(Something));
        Assert.That(Invalid.OrFallback("xxx"), Is.EqualTo("xxx"));
        Assert.That(Invalid.OrFallback(() => "xxx"), Is.EqualTo("xxx"));
        Assert.Throws<InvalidOperationException>(() => Invalid.OrThrow());
    }
    
    [Test]
    public void Should_be_able_to_use_monad_methods()
    {
        // Assert
        Assert.That(Valid.Map(ToUpper).OrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That(Valid.Bind(ValidatedToUpper).OrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That(Invalid.Map(ToUpper).OrFallback(() => null!), Is.EqualTo(null));
        Assert.That(Invalid.Bind(ValidatedToUpper).OrFallback(() => null!), Is.EqualTo(null));
    }
    
    [Test]
    public async Task Should_be_able_to_handle_async_lambdas()
    {
        // Assert
        Assert.That((await Valid.MapAsync(ToUpperAsync)).OrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That((await Valid.BindAsync(ValidatedToUpperAsync)).OrThrow(), Is.EqualTo(Something.ToUpper()));
        Assert.That((await Invalid.MapAsync(ToUpperAsync)).OrFallback(() => null!), Is.Null);
        Assert.That((await Invalid.BindAsync(ValidatedToUpperAsync)).OrFallback(() => null!), Is.Null);
    }
    
    [Test]
    public void Should_be_able_to_deconstruct()
    {
        // Act
        var (isValid1, value1, issues1) = Valid;
        var (isValid2, value2, issues2) = Invalid;

        // Assert
        Assert.That(isValid1, Is.True);
        Assert.That(value1, Is.EqualTo(Something));
        Assert.That(issues1, Is.Null);
        Assert.That(isValid2, Is.False);
        Assert.That(value2, Is.EqualTo(null));
        Assert.That(issues2.Count, Is.EqualTo(1));
        Assert.That(issues2.First(), Is.EqualTo(DefaultIssue));
    }
}