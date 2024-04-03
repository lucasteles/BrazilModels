namespace BrazilModels.Tests;

[TestFixture]
public class CnpjTests
{
    [Test]
    public void NewCnpjShouldBeEmpty() =>
        new Cnpj().Should().Be(Cnpj.Empty);

    [Test]
    public void ShouldHaveEmptyFormattedCnpj() =>
        Cnpj.Empty.ToString(withMask: true).Should().Be("00.000.000/0000-00");

    [Test]
    public void ShouldHaveEmptyCnpj() =>
        Cnpj.Empty.ToString().Should().Be("00000000000000");

    [PropertyTest]
    public void ShouldFormatFull(FormattedCnpj cnpj) =>
        new Cnpj(cnpj.Cleared).ToString(withMask: true).Should().Be(cnpj.Value);

    [PropertyTest]
    public void ShouldFormatClean(CleanCnpj cnpj)
    {
        var sut = new Cnpj(cnpj);
        sut.ToString(withMask: false)
            .Should().Be(cnpj.Cleared)
            .And.Be(sut.ToString());
    }

    [PropertyTest]
    public void ShouldBeImplicitString(FormattedCnpj input)
    {
        string cnpj = new Cnpj(input);
        cnpj.Should().Be(input.Cleared);
    }

    [PropertyTest]
    public void ShouldBeImplicitSpan(FormattedCnpj input)
    {
        ReadOnlySpan<char> cnpj = new Cnpj(input);
        cnpj.ToString().Should().Be(input.Cleared);
    }

    [PropertyTest]
    public void ShouldCompareAsString(ValidCnpj first, ValidCnpj second)
    {
        var cnpj1 = new Cnpj(first);
        var cnpj2 = new Cnpj(second);
        var strCompare = string.Compare(first.Cleared, second.Cleared,
            StringComparison.OrdinalIgnoreCase);
        cnpj1.CompareTo(cnpj2).Should().Be(strCompare);
    }

    public class NewTests
    {
        [PropertyTest]
        public void ShouldCreateACnpj(ValidCnpj cnpj) =>
            new Cnpj(cnpj).Value.Should().Be(cnpj.Cleared);

        [PropertyTest]
        public void ShouldCreateFormattedCnpj(FormattedCnpj cnpj) =>
            new Cnpj(cnpj).Value.Should().Be(cnpj.Cleared);

        [PropertyTest]
        public void ShouldCreateUnformattedCnpj(CleanCnpj cnpj) =>
            new Cnpj(cnpj).Value.Should().Be(cnpj.Cleared);

        [PropertyTest]
        public void ShouldThrowInvalidFormattedCnpj(InvalidCnpj cnpj)
        {
            var action = () => new Cnpj(cnpj);
            action.Should().Throw<FormatException>();
        }

        [PropertyTest]
        public void ShouldThrowInvalidUnformattedCnpj(InvalidCnpj cnpj)
        {
            var action = () => new Cnpj(cnpj.Cleared);
            action.Should().Throw<FormatException>();
        }

        [TestCase("12345601")]
        [TestCase("123456797")]
        [TestCase("1234567890")]
        public void ShouldThrowLeftTrimmedCnpj(string cnpj)
        {
            var action = () => new Cnpj(cnpj);
            action.Should().Throw<FormatException>();
        }
    }

    public class ExplicitCastTests
    {
        [PropertyTest]
        public void ShouldCreateACnpj(ValidCnpj cnpj) =>
            ((Cnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

        [PropertyTest]
        public void ShouldCreateFormattedCnpj(FormattedCnpj cnpj) =>
            ((Cnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

        [PropertyTest]
        public void ShouldCreateUnformattedCnpj(CleanCnpj cnpj) =>
            ((Cnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

        [PropertyTest]
        public void ShouldThrowInvalidFormattedCnpj(InvalidCnpj cnpj)
        {
            var action = () => (Cnpj)cnpj.Value;
            action.Should().Throw<FormatException>();
        }

        [PropertyTest]
        public void ShouldThrowInvalidUnformattedCnpj(InvalidCnpj cnpj)
        {
            var action = () => (Cnpj)cnpj.Cleared;
            action.Should().Throw<FormatException>();
        }

        [TestCase("12345601")]
        [TestCase("123456797")]
        [TestCase("1234567890")]
        public void ShouldThrowLeftTrimmedCnpj(string cnpj)
        {
            var action = () => (Cnpj)cnpj;
            action.Should().Throw<FormatException>();
        }
    }

    public class ParseTests
    {
        [PropertyTest]
        public void ShouldCreateACnpj(ValidCnpj cnpj) =>
            Cnpj.Parse(cnpj.Value).Value.Should().Be(cnpj.Cleared);

        [PropertyTest]
        public void ShouldCreateFormattedCnpj(FormattedCnpj cnpj) =>
            ((Cnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

        [PropertyTest]
        public void ShouldCreateUnformattedCnpj(CleanCnpj cnpj) =>
            ((Cnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

        [PropertyTest]
        public void ShouldThrowInvalidFormattedCnpj(InvalidCnpj cnpj)
        {
            var action = () => (Cnpj)cnpj.Value;
            action.Should().Throw<FormatException>();
        }

        [PropertyTest]
        public void ShouldThrowInvalidUnformattedCnpj(InvalidCnpj cnpj)
        {
            var action = () => (Cnpj)cnpj.Cleared;
            action.Should().Throw<FormatException>();
        }

        [TestCase("1123456000101", "01.123.456/0001-01")]
        [TestCase("123456000149", "00.123.456/0001-49")]
        public void ShouldParseLeftTrimmedCnpj(string cnpj, string expected) =>
            Cnpj.Parse(cnpj).ToString(true).Should().Be(expected);
    }

    public class TryParseTests
    {
        [PropertyTest]
        public void ShouldCreateACnpj(ValidCnpj input)
        {
            Cnpj.TryParse(input.Value, out var cnpj).Should().BeTrue();
            cnpj.Value.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCreateFormattedCnpj(FormattedCnpj input)
        {
            Cnpj.TryParse(input.Value, out var cnpj).Should().BeTrue();
            cnpj.Value.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCreateUnformattedCnpj(CleanCnpj input)
        {
            Cnpj.TryParse(input.Trimmed, out var cnpj).Should().BeTrue();
            cnpj.Value.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldReturnFalseForInvalidFormattedCnpj(InvalidCnpj input)
        {
            Cnpj.TryParse(input.Value, out var cnpj).Should().BeFalse();
            cnpj.Value.Should().Be(Cnpj.Empty);
        }

        [Test]
        public void ShouldReturnFalseForNullString()
        {
            Cnpj.TryParse(null, out var cnpj).Should().BeFalse();
            cnpj.Value.Should().Be(Cnpj.Empty);
        }

        [PropertyTest]
        public void ShouldThrowInvalidUnformattedCnpj(InvalidCnpj input)
        {
            Cnpj.TryParse(input.Cleared, out var cnpj).Should().BeFalse();
            cnpj.Value.Should().Be(Cnpj.Empty);
        }

        [TestCase("1123456000101", "01.123.456/0001-01")]
        [TestCase("123456000149", "00.123.456/0001-49")]
        public void ShouldParseLeftTrimmedCnpj(string input, string expected)
        {
            Cnpj.TryParse(input, out var cnpj).Should().BeTrue();
            cnpj.ToString(withMask: true).Should().Be(expected);
        }
    }
}
