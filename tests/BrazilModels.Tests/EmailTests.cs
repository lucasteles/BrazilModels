namespace BrazilModels.Tests;

using System.Text.Json;

public class EmailTests : BaseTest
{
    [Test]
    public void ShouldReturnValidForValidEmail()
    {
        var emailStr = faker.Person.Email;
        Email.IsValid(emailStr).Should().BeTrue();
    }

    [Test]
    public void ShouldReturnFalseForInvalid()
    {
        var emailStr = faker.Person.FirstName;
        Email.IsValid(emailStr).Should().BeFalse();
    }

    [Test]
    public void ShouldParseEmail()
    {
        var emailStr = "teste@email.com";
        var sut = Email.Parse(emailStr);
        sut.Value.Should().Be(emailStr);
    }

    [Test]
    public void ShouldThrowInvalidEmail()
    {
        var emailStr = faker.Person.FirstName;
        var action = () => Email.Parse(emailStr);
        action.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void ShouldTryParseSuccessfully()
    {
        var emailStr = faker.Person.Email;
        Email.TryParse(emailStr, out var email).Should().BeTrue();
        email.Value.Should().Be(emailStr.ToLower());
    }

    [Test]
    public void ShouldTryParseReturnFalse()
    {
        var emailStr = faker.Person.FirstName;
        Email.TryParse(emailStr, out _).Should().BeFalse();
    }

    [Test]
    public void ShouldCompareEmail()
    {
        var emailStr = faker.Person.Email;
        Email emailLower = Email.Parse(emailStr.ToLowerInvariant());
        Email emailUpper = Email.Parse(emailStr.ToUpperInvariant());
        emailLower.Should().Be(emailUpper);
    }


    record Sut(Email Value);

    [Test]
    public void ShouldSerializeEmail()
    {
        var data = faker.Person.Email;
        var json = JsonSerializer.Serialize(new Sut(Email.Parse(data)));
        json.Should().Be(@$"{{""Value"":""{data.ToLowerInvariant()}""}}");
    }

    [Test]
    public void ShouldDeserializeEmail()
    {
        var value = Email.Parse(faker.Person.Email);
        var body = @$"{{""Value"":""{value.Value}""}}";
        var parsed = JsonSerializer.Deserialize<Sut>(body)!;
        var expected = Email.Parse(value);
        parsed.Value.Should().Be(expected);
    }
}
