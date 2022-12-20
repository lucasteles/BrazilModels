namespace BrazilModels.Tests;

public class ExtensionsTests
{
    [TestCase("6.059.776,36", 6_059_776.36)]
    [TestCase("20,21", 20.21)]
    [TestCase("300,32", 300.32)]
    [TestCase("1.100,10", 1_100.10)]
    [TestCase("0,00", 0)]
    [TestCase("-1,00", -1.00)]
    [TestCase("999.999.999,99", 999_999_999.99)]
    public void ParseDecimalBrazil(string stringValue, decimal expected) =>
        stringValue.TryParseDecimalBrazil().Should().Be(expected);


    [TestCase("6059776,36", 6_059_776.36)]
    [TestCase("20,21", 20.21)]
    [TestCase("300,32", 300.32)]
    [TestCase("1100,1", 1_100.10)]
    [TestCase("0", 0)]
    [TestCase("-1", -1.00)]
    [TestCase("999999999,99", 999_999_999.99)]
    public void BrazilDecimalStringRepresentationWithoutFormat(string expected, decimal value) =>
        value.ToBrazilString().Should().Be(expected);


    [TestCase("R$ 6.059.776,36", 6_059_776.36)]
    [TestCase("R$ 20,21", 20.21)]
    [TestCase("R$ 300,32", 300.32)]
    [TestCase("R$ 1.100,10", 1_100.10)]
    [TestCase("R$ 0,00", 0)]
    [TestCase("-R$ 1,00", -1.00)]
    [TestCase("R$ 99.999.999,99", 99_999_999.99)]
    public void BrazilDecimalStringRepresentation(string expected, decimal value) =>
        value.ToBrazilMoneyString().Should().Be(expected);

    [TestCase("6.059.776,36", 6_059_776.36)]
    [TestCase("20,21", 20.21)]
    [TestCase("300,32", 300.32)]
    [TestCase("1.100,10", 1_100.10)]
    [TestCase("0,00", 0)]
    [TestCase("-1,00", -1.00)]
    [TestCase("99.999.999,99", 99_999_999.99)]
    public void BrazilDecimalStringRepresentationWithoutSymbol(string expected, decimal value) =>
        value.ToBrazilMoneyString(false).Should().Be(expected);

}
