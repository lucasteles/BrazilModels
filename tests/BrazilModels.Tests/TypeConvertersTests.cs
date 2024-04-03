using System.ComponentModel;

#pragma warning disable S2187
namespace BrazilModels.Tests;

[TestFixture]
public class TypeConverterTests : BaseTest
{
    public class CpfTests
    {
        [Test]
        public void ConverterIdValidShouldBeTrueToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cpf));
            var cpf = faker.Person.Cpf();
            converter.IsValid(cpf).Should().BeTrue();
        }

        [Test]
        public void CanConvertFromShouldBeTrueToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cpf));
            converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();
        }

        [PropertyTest]
        public void ConvertFromShouldWorkForString(ValidCpf input)
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cpf));
            var cpf = new Cpf(input);
            converter
                .ConvertFrom(null, null, input.Value)
                .Should()
                .Be(cpf);
        }

        [Test]
        public void CanConvertToShouldBeTrueToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cpf));
            converter.CanConvertTo(null, typeof(string))
                .Should().BeTrue();
        }

        [PropertyTest]
        public void ConvertToShouldWorkString(ValidCpf input)
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cpf));
            var cpf = new Cpf(input);
            converter
                .ConvertTo(null, null, cpf, typeof(string))
                .Should()
                .Be(input.Cleared);
        }
    }

    public class CnpjTests
    {
        [Test]
        public void ConverterIdValidShouldBeTrueToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cnpj));
            var cnpj = faker.Company.Cnpj();
            converter.IsValid(cnpj).Should().BeTrue();
        }

        [Test]
        public void CanConvertFromShouldBeTrueToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cnpj));
            converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();
        }

        [PropertyTest]
        public void ConvertFromShouldWorkForString(ValidCnpj input)
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cnpj));
            var cnpj = new Cnpj(input);
            converter
                .ConvertFrom(null, null, input.Value)
                .Should()
                .Be(cnpj);
        }

        [Test]
        public void CanConvertToShouldBeTrueToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cnpj));
            converter.CanConvertTo(null, typeof(string))
                .Should().BeTrue();
        }

        [PropertyTest]
        public void ConvertToShouldWorkString(ValidCnpj input)
        {
            var converter = TypeDescriptor.GetConverter(typeof(Cnpj));
            var cnpj = new Cnpj(input);
            converter
                .ConvertTo(null, null, cnpj, typeof(string))
                .Should()
                .Be(input.Cleared);
        }
    }

    public class CpfCnpjTests
    {
        [Test]
        public void ConverterIdValidShouldBeTrueToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(CpfCnpj));
            var cnpj = faker.Company.Cnpj();
            converter.IsValid(cnpj).Should().BeTrue();
        }

        [Test]
        public void CanConvertFromShouldBeTrueToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(CpfCnpj));
            converter.CanConvertFrom(null, typeof(string)).Should().BeTrue();
        }

        [PropertyTest]
        public void ConvertFromShouldWorkForString(ValidCnpj input)
        {
            var converter = TypeDescriptor.GetConverter(typeof(CpfCnpj));
            var cnpj = new CpfCnpj(input);
            converter
                .ConvertFrom(null, null, input.Value)
                .Should()
                .Be(cnpj);
        }

        [Test]
        public void CanConvertToShouldBeTrueToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(CpfCnpj));
            converter.CanConvertTo(null, typeof(string))
                .Should().BeTrue();
        }

        [PropertyTest]
        public void ConvertToShouldWorkString(ValidCnpj input)
        {
            var converter = TypeDescriptor.GetConverter(typeof(CpfCnpj));
            var cnpj = new CpfCnpj(input);
            converter
                .ConvertTo(null, null, cnpj, typeof(string))
                .Should()
                .Be(input.Cleared);
        }
    }
}
