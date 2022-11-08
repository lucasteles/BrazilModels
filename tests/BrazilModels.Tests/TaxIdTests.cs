namespace BrazilModels.Tests;

public class TaxIdTests
{
    [Test]
    public void ShouldHaveEmptyFormattedTaxId() =>
        TaxId.Empty.ToString(withMask: true).Should().Be(string.Empty);

    [Test]
    public void ShouldHaveEmptyTaxId() =>
        TaxId.Empty.ToString().Should().Be(string.Empty);

    [Test]
    public void NewTaxIdShouldBeEmpty() =>
        new TaxId().Should().Be(TaxId.Empty);

    public class TaxIdCnpjTests
    {
        [PropertyTest]
        public void ShouldHaveCnpjType(ValidCnpj cnpj) =>
            new TaxId(cnpj).Type.Should().Be(TaxIdType.CNPJ);

        [PropertyTest]
        public void ShouldFormatFull(FormattedCnpj cnpj) =>
            new TaxId(cnpj.Cleared).ToString(withMask: true).Should().Be(cnpj.Value);

        [PropertyTest]
        public void ShouldFormatClean(CleanCnpj cnpj)
        {
            var sut = new TaxId(cnpj);
            sut.ToString(withMask: false)
                .Should().Be(cnpj.Cleared)
                .And.Be(sut.ToString());
        }

        [PropertyTest]
        public void ShouldBeImplicitString(FormattedCnpj input)
        {
            string cnpj = new TaxId(input);
            cnpj.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldBeImplicitSpan(FormattedCnpj input)
        {
            ReadOnlySpan<char> cnpj = new TaxId(input);
            cnpj.ToString().Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCompareAsString(ValidCnpj first, ValidCnpj second)
        {
            var cnpj1 = new TaxId(first);
            var cnpj2 = new TaxId(second);
            var strCompare = string.Compare(first.Cleared, second.Cleared, StringComparison.OrdinalIgnoreCase);
            cnpj1.CompareTo(cnpj2).Should().Be(strCompare);
        }

        public class NewTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj cnpj) =>
                new TaxId(cnpj).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj cnpj) =>
                new TaxId(cnpj).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj cnpj) =>
                new TaxId(cnpj).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => new TaxId(cnpj);
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => new TaxId(cnpj.Cleared);
                action.Should().Throw<FormatException>();
            }
        }

        public class ExplicitCastTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj cnpj) =>
                ((TaxId)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj cnpj) =>
                ((TaxId)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj cnpj) =>
                ((TaxId)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => (TaxId)cnpj.Value;
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => (TaxId)cnpj.Cleared;
                action.Should().Throw<FormatException>();
            }
        }

        public class ParseTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj cnpj) =>
                TaxId.Parse(cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj cnpj) =>
                ((TaxId)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj cnpj) =>
                ((TaxId)cnpj.Value).Value.Should().Be(cnpj.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => (TaxId)cnpj.Value;
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCnpj cnpj)
            {
                var action = () => (TaxId)cnpj.Cleared;
                action.Should().Throw<FormatException>();
            }
        }

        public class TryParseTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCnpj input)
            {
                TaxId.TryParse(input.Value, out var cnpj).Should().BeTrue();
                cnpj.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCnpj input)
            {
                TaxId.TryParse(input.Value, out var cnpj).Should().BeTrue();
                cnpj.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCnpj input)
            {
                TaxId.TryParse(input.Cleared, out var cnpj).Should().BeTrue();
                cnpj.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCnpj input)
            {
                TaxId.TryParse(input.Value, out var cnpj).Should().BeFalse();
                cnpj.Value.Should().Be(TaxId.Empty);
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCnpj input)
            {
                TaxId.TryParse(input.Cleared, out var cnpj).Should().BeFalse();
                cnpj.Value.Should().Be(TaxId.Empty);
            }
        }
    }

    public class TaxIdCpfTests
    {

        [PropertyTest]
        public void ShouldHaveCnpjType(ValidCpf cpf) =>
            new TaxId(cpf).Type.Should().Be(TaxIdType.CPF);

        [PropertyTest]
        public void ShouldFormatFull(FormattedCpf cpf) =>
            new TaxId(cpf.Cleared).ToString(withMask: true).Should().Be(cpf.Value);

        [PropertyTest]
        public void ShouldFormatClean(CleanCpf cpf)
        {
            var sut = new TaxId(cpf);
            sut.ToString(withMask: false)
                .Should().Be(cpf.Cleared)
                .And.Be(sut.ToString());
        }

        [PropertyTest]
        public void ShouldBeImplicitString(FormattedCpf input)
        {
            string cpf = new TaxId(input);
            cpf.Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldBeImplicitSpan(FormattedCpf input)
        {
            ReadOnlySpan<char> cpf = new TaxId(input);
            cpf.ToString().Should().Be(input.Cleared);
        }

        [PropertyTest]
        public void ShouldCompareAsString(ValidCpf first, ValidCpf second)
        {
            var cpf1 = new TaxId(first);
            var cpf2 = new TaxId(second);
            var strCompare = string.Compare(first.Cleared, second.Cleared, StringComparison.OrdinalIgnoreCase);
            cpf1.CompareTo(cpf2).Should().Be(strCompare);
        }

        public class NewTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf cpf) =>
                new TaxId(cpf).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf cpf) =>
                new TaxId(cpf).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf cpf) =>
                new TaxId(cpf).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCpf cpf)
            {
                var action = () => new TaxId(cpf);
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCpf cpf)
            {
                var action = () => new TaxId(cpf.Cleared);
                action.Should().Throw<FormatException>();
            }
        }

        public class ExplicitCastTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf cpf) =>
                ((TaxId)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf cpf) =>
                ((TaxId)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf cpf) =>
                ((TaxId)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCpf cpf)
            {
                var action = () => (TaxId)cpf.Value;
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCpf cpf)
            {
                var action = () => (TaxId)cpf.Cleared;
                action.Should().Throw<FormatException>();
            }
        }

        public class ParseTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf cpf) =>
                TaxId.Parse(cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf cpf) =>
                ((TaxId)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf cpf) =>
                ((TaxId)cpf.Value).Value.Should().Be(cpf.Cleared);

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCpf cpf)
            {
                var action = () => (TaxId)cpf.Value;
                action.Should().Throw<FormatException>();
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCpf cpf)
            {
                var action = () => (TaxId)cpf.Cleared;
                action.Should().Throw<FormatException>();
            }
        }

        public class TryParseTests
        {
            [PropertyTest]
            public void ShouldCreateATaxId(ValidCpf input)
            {
                TaxId.TryParse(input.Value, out var cpf).Should().BeTrue();
                cpf.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldCreateFormattedTaxId(FormattedCpf input)
            {
                TaxId.TryParse(input.Value, out var cpf).Should().BeTrue();
                cpf.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldCreateUnformattedTaxId(CleanCpf input)
            {
                TaxId.TryParse(input.Cleared, out var cpf).Should().BeTrue();
                cpf.Value.Should().Be(input.Cleared);
            }

            [PropertyTest]
            public void ShouldThrowInvalidFormattedTaxId(InvalidCpf input)
            {
                TaxId.TryParse(input.Value, out var cpf).Should().BeFalse();
                cpf.Value.Should().Be(TaxId.Empty);
            }

            [PropertyTest]
            public void ShouldThrowInvalidUnformattedTaxId(InvalidCpf input)
            {
                TaxId.TryParse(input.Cleared, out var cpf).Should().BeFalse();
                cpf.Value.Should().Be(TaxId.Empty);
            }
        }
    }
}
