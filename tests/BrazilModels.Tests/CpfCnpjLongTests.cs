namespace BrazilModels.Tests;

[TestFixture]
public class CpfCnpjLongTests
{
    [Test]
    public void ShouldThrowIfZero()
    {
        Action action = () => _ = new CpfCnpj(0, DocumentType.CPF);
        action.Should().Throw<Exception>();
    }

    public class CnpjTests
    {
        [PropertyTest]
        public void ShouldHaveCnpjType(ValidCnpj cnpj) =>
            new CpfCnpj(cnpj.Number, DocumentType.CNPJ).Type.Should().Be(DocumentType.CNPJ);

        [PropertyTest]
        public void ShouldFormatFull(FormattedCnpj cnpj) =>
            new CpfCnpj(cnpj.Number, DocumentType.CNPJ).ToString(withMask: true).Should()
                .Be(cnpj.Value);

        [PropertyTest]
        public void ShouldFormatClean(CleanCnpj cnpj)
        {
            var sut = new CpfCnpj(cnpj.Number, DocumentType.CNPJ);
            sut.ToString(withMask: false)
                .Should().Be(cnpj.Cleared)
                .And.Be(sut.ToString());
        }

        [PropertyTest]
        public void ShouldBeImplicitString(FormattedCnpj input)
        {
            string cnpj = new CpfCnpj(input.Number, DocumentType.CNPJ);
            cnpj.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldBeImplicitSpan(FormattedCnpj input)
        {
            ReadOnlySpan<char> cnpj = new CpfCnpj(input.Number, DocumentType.CNPJ);
            cnpj.ToString().Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCompareAsString(ValidCnpj first, ValidCnpj second)
        {
            var cnpj1 = new CpfCnpj(first.Number, DocumentType.CNPJ);
            var cnpj2 = new CpfCnpj(second.Number, DocumentType.CNPJ);
            var strCompare = string.Compare(first.Cleared, second.Cleared,
                StringComparison.OrdinalIgnoreCase);
            cnpj1.CompareTo(cnpj2).Should().Be(strCompare);
        }

        public class NewTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj cnpj) =>
                new CpfCnpj(cnpj.Number, DocumentType.CNPJ).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj cnpj) =>
                new CpfCnpj(cnpj.Number, DocumentType.CNPJ).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj cnpj) =>
                new CpfCnpj(cnpj.Number, DocumentType.CNPJ).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => new CpfCnpj(cnpj.Number, DocumentType.CNPJ);
                action.Should().Throw<FormatException>();
            }
        }

        public class ExplicitCastTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj cnpj) =>
                ((CpfCnpj)cnpj.Value).ToNumber().Should().Be(cnpj.Number);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj cnpj) =>
                ((CpfCnpj)cnpj.Value).ToNumber().Should().Be(cnpj.Number);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj cnpj) =>
                ((CpfCnpj)cnpj.Value).ToNumber().Should().Be(cnpj.Number);
        }
    }

    public class CpfTests
    {
        [PropertyTest]
        public void ShouldHaveCnpjType(ValidCpf cpf) =>
            new CpfCnpj(cpf.Number, DocumentType.CPF).Type.Should().Be(DocumentType.CPF);

        [PropertyTest]
        public void ShouldFormatFull(FormattedCpf cpf) =>
            new CpfCnpj(cpf.Number, DocumentType.CPF).ToString(withMask: true).Should()
                .Be(cpf.Value);

        [PropertyTest]
        public void ShouldFormatClean(CleanCpf cpf)
        {
            var sut = new CpfCnpj(cpf.Number, DocumentType.CPF);
            sut.ToString(withMask: false)
                .Should().Be(cpf.Cleared)
                .And.Be(sut.ToString());
        }

        [PropertyTest]
        public void ShouldBeImplicitString(FormattedCpf input)
        {
            string cpf = new CpfCnpj(input.Number, DocumentType.CPF);
            cpf.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldBeImplicitSpan(FormattedCpf input)
        {
            ReadOnlySpan<char> cpf = new CpfCnpj(input.Number, DocumentType.CPF);
            cpf.ToString().Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCompareAsString(ValidCpf first, ValidCpf second)
        {
            var cpf1 = new CpfCnpj(first.Number, DocumentType.CPF);
            var cpf2 = new CpfCnpj(second.Number, DocumentType.CPF);
            var strCompare = string.Compare(first.Cleared, second.Cleared,
                StringComparison.OrdinalIgnoreCase);
            cpf1.CompareTo(cpf2).Should().Be(strCompare);
        }

        public class NewTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf cpf) =>
                new CpfCnpj(cpf.Number, DocumentType.CPF).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf cpf) =>
                new CpfCnpj(cpf.Number, DocumentType.CPF).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf cpf) =>
                new CpfCnpj(cpf.Number, DocumentType.CPF).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCpf cpf)
            {
                var action = () => new CpfCnpj(cpf.Number, DocumentType.CPF);
                action.Should().Throw<FormatException>();
            }
        }

        public class ExplicitCastTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf cpf) =>
                ((CpfCnpj)cpf.Value).ToNumber().Should().Be(cpf.Number);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf cpf) =>
                ((CpfCnpj)cpf.Value).ToNumber().Should().Be(cpf.Number);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf cpf) =>
                ((CpfCnpj)cpf.Value).ToNumber().Should().Be(cpf.Number);
        }
    }
}
