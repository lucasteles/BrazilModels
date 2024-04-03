using System.Text.RegularExpressions;
using FsCheck;

namespace BrazilModels.Tests.Utils;

public sealed class PropertyTestAttribute : FsCheck.NUnit.PropertyAttribute
{
    public PropertyTestAttribute() => this.QuietOnSuccess = true;
}

public abstract record DocumentValue(string Value)
{
    public static implicit operator string(DocumentValue document) => document.Value;
    public static implicit operator ReadOnlySpan<char>(DocumentValue document) => document.Value;

    public override string ToString() => Value;

    public string Cleared => Regex.Replace(Value, @"[\-\/\.]", string.Empty);
    public string Trimmed => Cleared.TrimStart('0');
}

public record FormattedCpf(string Cpf) : DocumentValue(Cpf);

public record CleanCnpj(string Cnpj) : DocumentValue(Cnpj);

public record ValidCnpj(string Cnpj) : DocumentValue(Cnpj);

public record FormattedCnpj(string Cnpj) : DocumentValue(Cnpj);

public record CleanCpf(string Cpf) : DocumentValue(Cpf);

public record ValidCpf(string Cpf) : DocumentValue(Cpf);

public record InvalidCpf(string Value) : DocumentValue(Value);

public record InvalidCnpj(string Value) : DocumentValue(Value);

public class CustomGenerators
{
    protected CustomGenerators()
    {
    }

    static Arbitrary<T> FakerGenerator<T>(Func<Faker, T> factory) where T : class
    {
        var generator = Gen.Sized(size =>
        {
            Faker faker = new("pt_BR");
            var cpfs = Enumerable.Range(0, size + 1)
                .Select(_ => factory(faker))
                .ToArray();
            return from i in Gen.Choose(0, size) select cpfs[i];
        });

        return Arb.From(generator);
    }

    public static Arbitrary<FormattedCpf> ChooseFormattedCpf() =>
        FakerGenerator(faker => new FormattedCpf(faker.Person.Cpf(true)));

    public static Arbitrary<FormattedCnpj> ChooseFormattedCpnj() =>
        FakerGenerator(faker => new FormattedCnpj(faker.Company.Cnpj(true)));

    public static Arbitrary<CleanCnpj> ChooseCleanCnpj() =>
        FakerGenerator(faker => new CleanCnpj(faker.Company.Cnpj(false)));

    public static Arbitrary<CleanCpf> ChooseCleanCpf() =>
        FakerGenerator(faker => new CleanCpf(faker.Person.Cpf(false)));

    public static Arbitrary<ValidCnpj> ChooseValidCnpj() =>
        FakerGenerator(faker => new ValidCnpj(faker.Company.Cnpj(faker.Random.Bool())));

    public static Arbitrary<ValidCpf> ChooseValidCpf() =>
        FakerGenerator(faker => new ValidCpf(faker.Person.Cpf(faker.Random.Bool())));

    static string Invalidate(Faker faker, string validDoc)
    {
        var valid = validDoc.ToCharArray();
        var index = faker.Random.Bool() ? ^1 : ^2;
        valid[index] = (char)(valid[index] + 1) is var next && char.IsDigit(next) ? next : '0';
        var invalid = new string(valid);
        return invalid;
    }

    public static Arbitrary<InvalidCnpj> ChooseInvalidCnpj() =>
        FakerGenerator(faker => new InvalidCnpj(Invalidate(faker, faker.Company.Cnpj())));

    public static Arbitrary<InvalidCpf> ChooseInvalidCpf() =>
        FakerGenerator(faker => new InvalidCpf(Invalidate(faker, faker.Person.Cpf())));
}
