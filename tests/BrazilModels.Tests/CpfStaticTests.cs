namespace BrazilModels.Tests;

public class CpfStaticTests
{
    [TestCase("12345601", "000.123.456-01")]
    [TestCase("123456797", "001.234.567-97")]
    [TestCase("1234567890", "012.345.678-90")]
    [TestCase("00012345601", "000.123.456-01")]
    [TestCase("00123456797", "001.234.567-97")]
    [TestCase("01234567890", "012.345.678-90")]
    [TestCase("99912345606", "999.123.456-06")]
    [TestCase("31981812083", "319.818.120-83")]
    public void ShouldFormatFull(string cpf, string expected) =>
        Cpf.Format(cpf, withMask: true).Should().Be(expected);

    [TestCase("000.123.456-01", "00012345601")]
    [TestCase("001.234.567-97", "00123456797")]
    [TestCase("012.345.678-90", "01234567890")]
    [TestCase("999.123.456-06", "99912345606")]
    [TestCase("319.818.120-83", "31981812083")]
    public void ShouldFormatClean(string cpf, string expected) =>
        Cpf.Format(cpf).Should().Be(expected);

    [PropertyTest]
    public void ShouldAlwaysFormatFull(FormattedCpf cpf) =>
        Cpf.Format(cpf.Trimmed, true).Should().Be(cpf);

    [PropertyTest]
    public void ShouldAlwaysBeSize14(FormattedCpf cpf) =>
        Cpf.Format(cpf).Should().HaveLength(11);

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("          ")]
    public void ShouldValidateEmpty(string input) =>
        Cpf.Validate(input).Should().BeFalse();

    [PropertyTest]
    public void ShouldValidateAnyCpf(ValidCpf cpf) =>
        Cpf.Validate(cpf).Should().BeTrue();

    [PropertyTest]
    public void ShouldValidateMasked(FormattedCpf cpf) =>
        Cpf.Validate(cpf).Should().BeTrue();

    [PropertyTest]
    public void ShouldValidate(CleanCpf cpf) =>
        Cpf.Validate(cpf).Should().BeTrue();

    [PropertyTest]
    public void ShouldValidateInvalidMasked(InvalidCpf cpf) =>
        Cpf.Validate(cpf).Should().BeFalse();

    [PropertyTest]
    public void ShouldValidateInvalid(InvalidCpf cpf) =>
        Cpf.Validate(cpf.Trimmed).Should().BeFalse();

    static IEnumerable<string> CpfsWithRepeatingDigits() =>
        Util.RepeatingDigits("###.###.###-##");

    [TestCaseSource(nameof(CpfsWithRepeatingDigits))]
    public void ShouldValidateAsInvalidWhenSameDigits(string cpf) =>
        Cpf.Validate(cpf).Should().BeFalse();
}
