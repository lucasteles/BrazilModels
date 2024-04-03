using System.Text.RegularExpressions;

namespace BrazilModels.Tests;

using System.Text.Json;

public class PhoneNumberTests : BaseTest
{
    [Test]
    public void ShouldReturnValidForValidPhoneNumber()
    {
        var phoneNumberStr = faker.Person.Phone;
        PhoneNumber.TryParse(phoneNumberStr, out _).Should().BeTrue();
    }

    [Test]
    public void ShouldReturnFalseForInvalid() =>
        PhoneNumber.TryParse(string.Empty, out _).Should().BeFalse();

    [Test]
    public void ShouldParsePhoneNumberWithCountryCode()
    {
        const string phoneNumber = "+55(11) 91234-5678";
        const string phoneNumberClean = "+5511912345678";

        var sut = PhoneNumber.Parse(phoneNumber);
        sut.Value.Should().Be(phoneNumberClean);
    }

    [Test]
    public void ShouldParsePhoneNumber()
    {
        const string phoneNumber = "(11) 91234-5678";
        const string phoneNumberClean = "11912345678";

        var sut = PhoneNumber.Parse(phoneNumber);
        sut.Value.Should().Be(phoneNumberClean);
    }


    [Test]
    public void ShouldComparePhoneNumber()
    {
        var phone1 = PhoneNumber.Parse("(11) 91234-5678");
        var phone2 = PhoneNumber.Parse("11912345678");
        phone1.Should().Be(phone2);
    }

    record Sut(PhoneNumber Value);

    static string Clear(string input) =>
        Regex.Replace(input, "[^0-9a-zA-Z+]+", string.Empty).Trim();


    [Test]
    public void ShouldSerializePhoneNumber()
    {
        var data = faker.Person.Phone;
        var json = JsonSerializer.Serialize(new Sut(PhoneNumber.Parse(data)),
            new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            });

        json.Should().Be(@$"{{""Value"":""{Clear(data)}""}}");
    }

    [Test]
    public void ShouldSerializePhoneNumberWithPlus()
    {
        const string data = "+55 (80) 2640-4542";
        var json = JsonSerializer.Serialize(new Sut(PhoneNumber.Parse(data)),
            new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            });

        json.Should().Be(@$"{{""Value"":""+558026404542""}}");
    }

    [Test]
    public void ShouldDeserializePhoneNumber()
    {
        var value = PhoneNumber.Parse(faker.Person.Phone);
        var body = @$"{{""Value"":""{value.Value}""}}";
        var parsed = JsonSerializer.Deserialize<Sut>(body)!;
        var expected = PhoneNumber.Parse(value);
        parsed.Value.Should().Be(expected);
    }
}
