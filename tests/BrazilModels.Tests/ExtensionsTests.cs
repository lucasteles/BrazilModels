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
    public void NewCpfShouldBeEmpty(string stringValue, decimal expected) =>
        stringValue.TryParseDecimalBrazil().Should().Be(expected);
}
