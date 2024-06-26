using System.Text.Json;

namespace BrazilModels.Tests;

public class SerializeTests
{
    record Sut<T>(T Value);

    [PropertyTest]
    public void ShouldSerializeCnpj(ValidCnpj data)
    {
        var json = JsonSerializer.Serialize(new Sut<Cnpj>(new(data)));
        json.Should().Be(@$"{{""Value"":""{data.Cleared}""}}");
    }

    [PropertyTest]
    public void ShouldSerializeCpf(ValidCpf data)
    {
        var json = JsonSerializer.Serialize(new Sut<Cpf>(new(data)));
        json.Should().Be(@$"{{""Value"":""{data.Cleared}""}}");
    }

    [PropertyTest]
    public void ShouldDeserializeCnpj(ValidCnpj value)
    {
        var body = @$"{{""Value"":""{value.Cleared}""}}";
        var parsed = JsonSerializer.Deserialize<Sut<Cnpj>>(body)!;
        var expected = new Cnpj(value);
        parsed.Value.Should().Be(expected);
    }

    [PropertyTest]
    public void ShouldDeserializeCpf(ValidCpf value)
    {
        var body = @$"{{""Value"":""{value.Cleared}""}}";
        var parsed = JsonSerializer.Deserialize<Sut<Cpf>>(body)!;
        var expected = new Cpf(value);
        parsed.Value.Should().Be(expected);
    }


    [PropertyTest]
    public void ShouldSerializeTaxIdCnpj(ValidCnpj data)
    {
        var json = JsonSerializer.Serialize(new Sut<CpfCnpj>(new(data)));
        json.Should().Be(@$"{{""Value"":""{data.Cleared}""}}");
    }

    [PropertyTest]
    public void ShouldSerializeTaxIdCpf(ValidCpf data)
    {
        var json = JsonSerializer.Serialize(new Sut<CpfCnpj>(new(data)));
        json.Should().Be(@$"{{""Value"":""{data.Cleared}""}}");
    }

    [PropertyTest]
    public void ShouldDeserializeTaxIdCnpj(ValidCnpj value)
    {
        var body = @$"{{""Value"":""{value.Cleared}""}}";
        var parsed = JsonSerializer.Deserialize<Sut<CpfCnpj>>(body)!;
        var expected = new CpfCnpj(value);
        parsed.Value.Should().Be(expected);
    }

    [PropertyTest]
    public void ShouldDeserializeTaxIdCpf(ValidCpf value)
    {
        var body = @$"{{""Value"":""{value.Cleared}""}}";
        var parsed = JsonSerializer.Deserialize<Sut<CpfCnpj>>(body)!;
        var expected = new CpfCnpj(value);
        parsed.Value.Should().Be(expected);
    }
}
