namespace BrazilModels.Tests;

public class CnpjStaticTests
{
    [TestCase("49.020.406/0001-25", "49.020.406/0001-25")]
    [TestCase("1.123.456/0001-01", "01.123.456/0001-01")]
    [TestCase("123.456/0001-49", "00.123.456/0001-49")]
    [TestCase("49020406000125", "49.020.406/0001-25")]
    [TestCase("1123456000101", "01.123.456/0001-01")]
    [TestCase("123456000149", "00.123.456/0001-49")]
    public void ShouldFormatFull(string cnpj, string expected) =>
        Cnpj.Format(cnpj, true).Should().Be(expected);

    [TestCase("49.020.406/0001-25", "49020406000125")]
    [TestCase("7.3285.396/0001-34", "73285396000134")]
    [TestCase("01.123.456/0001-01", "01123456000101")]
    [TestCase("00.123.456/0001-49", "00123456000149")]
    public void ShouldFormatClean(string cnpj, string expected) =>
        Cnpj.Format(cnpj).Should().Be(expected);

    [PropertyTest]
    public void ShouldAlwaysFormatFull(FormattedCnpj cnpj) =>
        Cnpj.Format(cnpj.Cleared, true).Should().Be(cnpj);

    [PropertyTest]
    public void ShouldAlwaysBeSize14(FormattedCnpj cnpj) =>
        Cnpj.Format(cnpj).Should().HaveLength(14);

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("          ")]
    public void ShouldValidateEmpty(string input) =>
        Cnpj.Validate(input).Should().BeFalse();

    [PropertyTest]
    public void ShouldValidateAnyCpnj(ValidCnpj cnpj) =>
        Cnpj.Validate(cnpj).Should().BeTrue();

    [PropertyTest]
    public void ShouldValidateMasked(FormattedCnpj cnpj) =>
        Cnpj.Validate(cnpj).Should().BeTrue();

    [PropertyTest]
    public void ShouldValidateMasked(CleanCnpj cnpj) =>
        Cnpj.Validate(cnpj).Should().BeTrue();

    [PropertyTest]
    public void ShouldValidateInvalidMasked(InvalidCnpj cnpj) =>
        Cnpj.Validate(cnpj).Should().BeFalse();

    [PropertyTest]
    public void ShouldValidateAsInvalid(InvalidCnpj cnpj) =>
        Cnpj.Validate(cnpj.Cleared).Should().BeFalse();

    static IEnumerable<string> CnpjsWithRepeatingDigits() =>
        Util.RepeatingDigits("##.###.###/####-##");

    [TestCaseSource(nameof(CnpjsWithRepeatingDigits))]
    public void ShouldValidateAsInvalidWhenSameDigits(string cnpj) =>
        Cnpj.Validate(cnpj).Should().BeFalse();
}
