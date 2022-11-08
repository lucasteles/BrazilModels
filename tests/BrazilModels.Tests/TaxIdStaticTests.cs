namespace BrazilModels.Tests;

public class TaxIdStaticTests
{
    public class TaxIdAsCnpjStaticTests
    {
        [TestCase("49.020.406/0001-25", "49.020.406/0001-25")]
        [TestCase("01.123.456/0001-01", "01.123.456/0001-01")]
        [TestCase("00.123.456/0001-49", "00.123.456/0001-49")]
        [TestCase("49020406000125", "49.020.406/0001-25")]
        [TestCase("01123456000101", "01.123.456/0001-01")]
        [TestCase("00123456000149", "00.123.456/0001-49")]
        public void ShouldFormatFull(string cnpj, string expected) =>
            TaxId.Format(cnpj, true).Should().Be(expected);

        [TestCase("49.020.406/0001-25", "49020406000125")]
        [TestCase("7.3285.396/0001-34", "73285396000134")]
        [TestCase("01.123.456/0001-01", "01123456000101")]
        [TestCase("00.123.456/0001-49", "00123456000149")]
        public void ShouldFormatClean(string cnpj, string expected) =>
            TaxId.Format(cnpj).Should().Be(expected);

        [PropertyTest]
        public void ShouldAlwaysFormatFull(FormattedCnpj cnpj) =>
            TaxId.Format(cnpj.Cleared, true).Should().Be(cnpj);

        [PropertyTest]
        public void ShouldAlwaysBeSize14(FormattedCnpj cnpj) =>
            TaxId.Format(cnpj).Should().HaveLength(14);

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("          ")]
        public void ShouldValidateEmpty(string input) =>
            TaxId.Validate(input).Should().BeNull();

        [PropertyTest]
        public void ShouldValidateAnyCpnj(ValidCnpj cnpj) =>
            TaxId.Validate(cnpj).Should().Be(TaxIdType.CNPJ);

        [PropertyTest]
        public void ShouldValidateMasked(FormattedCnpj cnpj) =>
            TaxId.Validate(cnpj).Should().Be(TaxIdType.CNPJ);

        [PropertyTest]
        public void ShouldValidateMasked(CleanCnpj cnpj) =>
            TaxId.Validate(cnpj).Should().Be(TaxIdType.CNPJ);

        [PropertyTest]
        public void ShouldValidateInvalidMasked(InvalidCnpj cnpj) =>
            TaxId.Validate(cnpj).Should().NotBe(TaxIdType.CNPJ);

        [PropertyTest]
        public void ShouldValidateAsInvalid(InvalidCnpj cnpj) =>
            TaxId.Validate(cnpj.Cleared).Should().NotBe(TaxIdType.CNPJ);

        static IEnumerable<string> CnpjsWithRepeatingDigits() =>
            Util.RepeatingDigits("##.###.###/####-##");

        [TestCaseSource(nameof(CnpjsWithRepeatingDigits))]
        public void ShouldValidateAsInvalidWhenSameDigits(string cnpj) =>
            TaxId.Validate(cnpj).Should().BeNull();
    }

    public class TaxIdCpfStaticTests
    {
        [TestCase("00012345601", "000.123.456-01")]
        [TestCase("00123456797", "001.234.567-97")]
        [TestCase("01234567890", "012.345.678-90")]
        [TestCase("00012345601", "000.123.456-01")]
        [TestCase("00123456797", "001.234.567-97")]
        [TestCase("01234567890", "012.345.678-90")]
        [TestCase("99912345606", "999.123.456-06")]
        [TestCase("31981812083", "319.818.120-83")]
        public void ShouldFormatFull(string cpf, string expected) =>
            TaxId.Format(cpf, withMask: true).Should().Be(expected);

        [TestCase("000.123.456-01", "00012345601")]
        [TestCase("001.234.567-97", "00123456797")]
        [TestCase("012.345.678-90", "01234567890")]
        [TestCase("999.123.456-06", "99912345606")]
        [TestCase("319.818.120-83", "31981812083")]
        public void ShouldFormatClean(string cpf, string expected) =>
            TaxId.Format(cpf).Should().Be(expected);

        [PropertyTest]
        public void ShouldAlwaysFormatFull(FormattedCpf cpf) =>
            TaxId.Format(cpf.Cleared, true).Should().Be(cpf);

        [PropertyTest]
        public void ShouldAlwaysBeSize14(FormattedCpf cpf) =>
            TaxId.Format(cpf).Should().HaveLength(11);

        [PropertyTest]
        public void ShouldValidateAnyCpf(ValidCpf cpf) =>
            TaxId.Validate(cpf).Should().Be(TaxIdType.CPF);

        [PropertyTest]
        public void ShouldValidateMasked(FormattedCpf cpf) =>
            TaxId.Validate(cpf).Should().Be(TaxIdType.CPF);

        [PropertyTest]
        public void ShouldValidate(CleanCpf cpf) =>
            TaxId.Validate(cpf).Should().Be(TaxIdType.CPF);

        [PropertyTest]
        public void ShouldValidateInvalidMasked(InvalidCpf cpf) =>
            TaxId.Validate(cpf).Should().NotBe(TaxIdType.CPF);

        [PropertyTest]
        public void ShouldValidateInvalid(InvalidCpf cpf) =>
            TaxId.Validate(cpf.Trimmed).Should().NotBe(TaxIdType.CPF);

        static IEnumerable<string> CpfsWithRepeatingDigits() =>
            Util.RepeatingDigits("###.###.###-##");

        [TestCaseSource(nameof(CpfsWithRepeatingDigits))]
        public void ShouldValidateAsInvalidWhenSameDigits(string cpf) =>
            TaxId.Validate(cpf).Should().BeNull();
    }
}
