namespace BrazilModels.Tests;

[TestFixture]
public class CpfTests
{
    [Test]
    public void NewCpfShouldBeEmpty() =>
        new Cpf().Should().Be(Cpf.Empty);

    [Test]
    public void ShouldHaveEmptyFormattedCpf() =>
        Cpf.Empty.ToString(withMask: true).Should().Be("000.000.000-00");

    [Test]
    public void ShouldHaveEmptyCpf() =>
        Cpf.Empty.ToString().Should().Be("00000000000");

    [PropertyTest]
    public void ShouldFormatFull(FormattedCpf cpf) =>
        new Cpf(cpf.Cleared).ToString(withMask: true).Should().Be(cpf.Value);

    [PropertyTest]
    public void ShouldFormatClean(CleanCpf cpf)
    {
        var sut = new Cpf(cpf);
        sut.ToString(withMask: false)
            .Should().Be(cpf.Cleared)
            .And.Be(sut.ToString());
    }

    [PropertyTest]
    public void ShouldBeImplicitString(FormattedCpf input)
    {
        string cpf = new Cpf(input);
        cpf.Should().Be(input.Cleared);
    }

    [PropertyTest]
    public void ShouldBeImplicitSpan(FormattedCpf input)
    {
        ReadOnlySpan<char> cpf = new Cpf(input);
        cpf.ToString().Should().Be(input.Cleared);
    }

    [PropertyTest]
    public void ShouldCompareAsString(ValidCpf first, ValidCpf second)
    {
        var cpf1 = new Cpf(first);
        var cpf2 = new Cpf(second);
        var strCompare = string.Compare(first.Cleared, second.Cleared,
            StringComparison.OrdinalIgnoreCase);
        cpf1.CompareTo(cpf2).Should().Be(strCompare);
    }

    public class NewTests
    {
        [PropertyTest]
        public void ShouldCreateACpf(ValidCpf cpf) =>
            new Cpf(cpf).Value.Should().Be(cpf.Cleared);

        [PropertyTest]
        public void ShouldCreateFormattedCpf(FormattedCpf cpf) =>
            new Cpf(cpf).Value.Should().Be(cpf.Cleared);

        [PropertyTest]
        public void ShouldCreateUnformattedCpf(CleanCpf cpf) =>
            new Cpf(cpf).Value.Should().Be(cpf.Cleared);

        [PropertyTest]
        public void ShouldThrowInvalidFormattedCpf(InvalidCpf cpf)
        {
            var action = () => new Cpf(cpf);
            action.Should().Throw<FormatException>();
        }

        [PropertyTest]
        public void ShouldThrowInvalidUnformattedCpf(InvalidCpf cpf)
        {
            var action = () => new Cpf(cpf.Cleared);
            action.Should().Throw<FormatException>();
        }

        [TestCase("12345601")]
        [TestCase("123456797")]
        [TestCase("1234567890")]
        public void ShouldThrowLeftTrimmedCpf(string cpf)
        {
            var action = () => new Cpf(cpf);
            action.Should().Throw<FormatException>();
        }
    }

    public class ExplicitCastTests
    {
        [PropertyTest]
        public void ShouldCreateACpf(ValidCpf cpf) =>
            ((Cpf)cpf.Value).Value.Should().Be(cpf.Cleared);

        [PropertyTest]
        public void ShouldCreateFormattedCpf(FormattedCpf cpf) =>
            ((Cpf)cpf.Value).Value.Should().Be(cpf.Cleared);

        [PropertyTest]
        public void ShouldCreateUnformattedCpf(CleanCpf cpf) =>
            ((Cpf)cpf.Value).Value.Should().Be(cpf.Cleared);

        [PropertyTest]
        public void ShouldThrowInvalidFormattedCpf(InvalidCpf cpf)
        {
            var action = () => (Cpf)cpf.Value;
            action.Should().Throw<FormatException>();
        }

        [PropertyTest]
        public void ShouldThrowInvalidUnformattedCpf(InvalidCpf cpf)
        {
            var action = () => (Cpf)cpf.Cleared;
            action.Should().Throw<FormatException>();
        }

        [TestCase("12345601")]
        [TestCase("123456797")]
        [TestCase("1234567890")]
        public void ShouldThrowLeftTrimmedCpf(string cpf)
        {
            var action = () => (Cpf)cpf;
            action.Should().Throw<FormatException>();
        }

        [TestCase(12345601)]
        [TestCase(123456797)]
        [TestCase(1234567890)]
        public void ShouldThrowLeftTrimmedCpf(long cpf)
        {
            var action = () => (Cpf)cpf;
            action.Should().Throw<FormatException>();
        }
    }

    public class ParseTests
    {
        [PropertyTest]
        public void ShouldCreateACpf(ValidCpf cpf) =>
            Cpf.Parse(cpf.Value).Value.Should().Be(cpf.Cleared);

        [PropertyTest]
        public void ShouldCreateFormattedCpf(FormattedCpf cpf) =>
            ((Cpf)cpf.Value).Value.Should().Be(cpf.Cleared);

        [PropertyTest]
        public void ShouldCreateUnformattedCpf(CleanCpf cpf) =>
            ((Cpf)cpf.Value).Value.Should().Be(cpf.Cleared);

        [PropertyTest]
        public void ShouldThrowInvalidFormattedCpf(InvalidCpf cpf)
        {
            var action = () => (Cpf)cpf.Value;
            action.Should().Throw<FormatException>();
        }

        [PropertyTest]
        public void ShouldThrowInvalidUnformattedCpf(InvalidCpf cpf)
        {
            var action = () => (Cpf)cpf.Cleared;
            action.Should().Throw<FormatException>();
        }

        [TestCase("12345601", "000.123.456-01")]
        [TestCase("123456797", "001.234.567-97")]
        [TestCase("1234567890", "012.345.678-90")]
        public void ShouldParseLeftTrimmedCpf(string cpf, string expected) =>
            Cpf.Parse(cpf).ToString(true).Should().Be(expected);

        [TestCase(12345601, "000.123.456-01")]
        [TestCase(123456797, "001.234.567-97")]
        [TestCase(1234567890, "012.345.678-90")]
        public void ShouldParseLeftTrimmedCpf(long cpf, string expected) =>
            Cpf.Parse(cpf).ToString(true).Should().Be(expected);
    }

    public class TryParseTests
    {
        [PropertyTest]
        public void ShouldCreateACpf(ValidCpf input)
        {
            Cpf.TryParse(input.Value, out var cpf).Should().BeTrue();
            cpf.Value.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCreateFormattedCpf(FormattedCpf input)
        {
            Cpf.TryParse(input.Value, out var cpf).Should().BeTrue();
            cpf.Value.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCreateUnformattedCpf(CleanCpf input)
        {
            Cpf.TryParse(input.Trimmed, out var cpf).Should().BeTrue();
            cpf.Value.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldReturnFalseForInvalidFormattedCpf(InvalidCpf input)
        {
            Cpf.TryParse(input.Value, out var cpf).Should().BeFalse();
            cpf.Value.Should().Be(Cpf.Empty);
        }

        [Test]
        public void ShouldReturnFalseForNullString()
        {
            Cpf.TryParse(null, out var cnpj).Should().BeFalse();
            cnpj.Value.Should().Be(Cpf.Empty);
        }

        [PropertyTest]
        public void ShouldThrowInvalidUnformattedCpf(InvalidCpf input)
        {
            Cpf.TryParse(input.Cleared, out var cpf).Should().BeFalse();
            cpf.Value.Should().Be(Cpf.Empty);
        }

        [TestCase("12345601", "000.123.456-01")]
        [TestCase("123456797", "001.234.567-97")]
        [TestCase("1234567890", "012.345.678-90")]
        public void ShouldParseLeftTrimmedCpf(string input, string expected)
        {
            Cpf.TryParse(input, out var cpf).Should().BeTrue();
            cpf.ToString(true).Should().Be(expected);
        }

        [TestCase(12345601, "000.123.456-01")]
        [TestCase(123456797, "001.234.567-97")]
        [TestCase(1234567890, "012.345.678-90")]
        public void ShouldParseLeftTrimmedCpf(long input, string expected)
        {
            Cpf.TryParse(input, out var cpf).Should().BeTrue();
            cpf.ToString(true).Should().Be(expected);
        }
    }
}
