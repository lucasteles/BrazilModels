namespace BrazilModels.Tests;

[TestFixture]
public class CpfCnpjTests
{
    [Test]
    public void ShouldHaveEmptyFormattedTaxId() =>
        CpfCnpj.Empty.ToString(withMask: true).Should().Be(string.Empty);

    [Test]
    public void ShouldHaveEmptyTaxId() =>
        CpfCnpj.Empty.ToString().Should().Be(string.Empty);

    [Test]
    public void NewTaxIdShouldBeEmpty() =>
        new CpfCnpj().Should().Be(CpfCnpj.Empty);

    public class CnpjTests
    {
        [PropertyTest]
        public void ShouldHaveCnpjType(ValidCnpj cnpj) =>
            new CpfCnpj(cnpj).Type.Should().Be(DocumentType.CNPJ);

        [PropertyTest]
        public void ShouldFormatFull(FormattedCnpj cnpj) =>
            new CpfCnpj(cnpj.Cleared).ToString(withMask: true).Should().Be(cnpj.Value);

        [PropertyTest]
        public void ShouldFormatClean(CleanCnpj cnpj)
        {
            var sut = new CpfCnpj(cnpj);
            sut.ToString(withMask: false)
                .Should().Be(cnpj.Cleared)
                .And.Be(sut.ToString());
        }

        [PropertyTest]
        public void ShouldBeImplicitString(FormattedCnpj input)
        {
            string cnpj = new CpfCnpj(input);
            cnpj.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldBeImplicitSpan(FormattedCnpj input)
        {
            ReadOnlySpan<char> cnpj = new CpfCnpj(input);
            cnpj.ToString().Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCompareAsString(ValidCnpj first, ValidCnpj second)
        {
            var cnpj1 = new CpfCnpj(first);
            var cnpj2 = new CpfCnpj(second);
            var strCompare = string.Compare(first.Cleared, second.Cleared,
                StringComparison.OrdinalIgnoreCase);
            cnpj1.CompareTo(cnpj2).Should().Be(strCompare);
        }

        public class NewTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj cnpj) =>
                new CpfCnpj(cnpj).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj cnpj) =>
                new CpfCnpj(cnpj).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj cnpj) =>
                new CpfCnpj(cnpj).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => new CpfCnpj(cnpj);
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => new CpfCnpj(cnpj.Cleared);
                action.Should().Throw<FormatException>();
            }
        }

        public class ExplicitCastTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj cnpj) =>
                ((CpfCnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj cnpj) =>
                ((CpfCnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj cnpj) =>
                ((CpfCnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => (CpfCnpj)cnpj.Value;
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => (CpfCnpj)cnpj.Cleared;
                action.Should().Throw<FormatException>();
            }
        }

        public class ParseTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj cnpj) =>
                CpfCnpj.Parse(cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj cnpj) =>
                ((CpfCnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj cnpj) =>
                ((CpfCnpj)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => (CpfCnpj)cnpj.Value;
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => (CpfCnpj)cnpj.Cleared;
                action.Should().Throw<FormatException>();
            }
        }

        public class TryParseTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj input)
            {
                CpfCnpj.TryParse(input.Value, out var cnpj).Should().BeTrue();
                cnpj.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj input)
            {
                CpfCnpj.TryParse(input.Value, out var cnpj).Should().BeTrue();
                cnpj.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj input)
            {
                CpfCnpj.TryParse(input.Cleared, out var cnpj).Should().BeTrue();
                cnpj.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldReturnFalseForInvalidFormattedTaxId(InvalidCnpj input)
            {
                CpfCnpj.TryParse(input.Value, out var cnpj).Should().BeFalse();
                cnpj.Value.Should().Be(CpfCnpj.Empty);
            }

            [Test]
            public void ShouldReturnFalseForNullString()
            {
                CpfCnpj.TryParse(null, out var cnpj).Should().BeFalse();
                cnpj.Value.Should().Be(CpfCnpj.Empty);
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCnpj input)
            {
                CpfCnpj.TryParse(input.Cleared, out var cnpj).Should().BeFalse();
                cnpj.Value.Should().Be(CpfCnpj.Empty);
            }
        }
    }

    public class CpfTests
    {
        [PropertyTest]
        public void ShouldHaveCnpjType(ValidCpf cpf) =>
            new CpfCnpj(cpf).Type.Should().Be(DocumentType.CPF);

        [PropertyTest]
        public void ShouldFormatFull(FormattedCpf cpf) =>
            new CpfCnpj(cpf.Cleared).ToString(withMask: true).Should().Be(cpf.Value);

        [PropertyTest]
        public void ShouldFormatClean(CleanCpf cpf)
        {
            var sut = new CpfCnpj(cpf);
            sut.ToString(withMask: false)
                .Should().Be(cpf.Cleared)
                .And.Be(sut.ToString());
        }

        [PropertyTest]
        public void ShouldBeImplicitString(FormattedCpf input)
        {
            string cpf = new CpfCnpj(input);
            cpf.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldBeImplicitSpan(FormattedCpf input)
        {
            ReadOnlySpan<char> cpf = new CpfCnpj(input);
            cpf.ToString().Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCompareAsString(ValidCpf first, ValidCpf second)
        {
            var cpf1 = new CpfCnpj(first);
            var cpf2 = new CpfCnpj(second);
            var strCompare = string.Compare(first.Cleared, second.Cleared,
                StringComparison.OrdinalIgnoreCase);
            cpf1.CompareTo(cpf2).Should().Be(strCompare);
        }

        public class NewTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf cpf) =>
                new CpfCnpj(cpf).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf cpf) =>
                new CpfCnpj(cpf).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf cpf) =>
                new CpfCnpj(cpf).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCpf cpf)
            {
                var action = () => new CpfCnpj(cpf);
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCpf cpf)
            {
                var action = () => new CpfCnpj(cpf.Cleared);
                action.Should().Throw<FormatException>();
            }
        }

        public class ExplicitCastTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf cpf) =>
                ((CpfCnpj)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf cpf) =>
                ((CpfCnpj)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf cpf) =>
                ((CpfCnpj)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCpf cpf)
            {
                var action = () => (CpfCnpj)cpf.Value;
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCpf cpf)
            {
                var action = () => (CpfCnpj)cpf.Cleared;
                action.Should().Throw<FormatException>();
            }
        }

        public class ParseTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf cpf) =>
                CpfCnpj.Parse(cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf cpf) =>
                ((CpfCnpj)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf cpf) =>
                ((CpfCnpj)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCpf cpf)
            {
                var action = () => (CpfCnpj)cpf.Value;
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCpf cpf)
            {
                var action = () => (CpfCnpj)cpf.Cleared;
                action.Should().Throw<FormatException>();
            }
        }

        public class TryParseTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf input)
            {
                CpfCnpj.TryParse(input.Value, out var cpf).Should().BeTrue();
                cpf.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf input)
            {
                CpfCnpj.TryParse(input.Value, out var cpf).Should().BeTrue();
                cpf.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf input)
            {
                CpfCnpj.TryParse(input.Cleared, out var cpf).Should().BeTrue();
                cpf.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCpf input)
            {
                CpfCnpj.TryParse(input.Value, out var cpf).Should().BeFalse();
                cpf.Value.Should().Be(CpfCnpj.Empty);
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCpf input)
            {
                CpfCnpj.TryParse(input.Cleared, out var cpf).Should().BeFalse();
                cpf.Value.Should().Be(CpfCnpj.Empty);
            }
        }
    }
}
