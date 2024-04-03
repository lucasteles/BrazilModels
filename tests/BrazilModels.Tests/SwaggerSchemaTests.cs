using BrazilModels.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BrazilModels.Tests;

public class SwaggerSchemaTests : BaseTest
{
    [TestCase(typeof(Cpf))]
    [TestCase(typeof(Cnpj))]
    [TestCase(typeof(CpfCnpj))]
    [TestCase(typeof(Email))]
    [TestCase(typeof(PhoneNumber))]
    public void CanShowImplementationTypeExampleWithSwaggerSchemaFilter(Type type)
    {
        SchemaGenerator schemaGenerator = new(new(), new JsonSerializerDataContractResolver(new()));
        SchemaRepository schemaRepository = new();

        var schema = schemaGenerator.GenerateSchema(type, schemaRepository);
        BrazilModelsSchemaFilter schemaFilter = new();
        schemaFilter.Apply(schema, new(type, schemaGenerator, schemaRepository));

        schema.Type.Should().Be("string");
        schema.Format.Should().Be(string.Empty);
        schema.Example.Should().BeNull();
    }


    [TestCase(typeof(int))]
    [TestCase(typeof(bool))]
    public void SkipShowImplementationTypeExampleForOtherTypes(Type type)
    {
        SchemaGenerator schemaGenerator = new(new(), new JsonSerializerDataContractResolver(new()));
        SchemaRepository schemaRepository = new();

        var schema = schemaGenerator.GenerateSchema(type, schemaRepository);
        BrazilModelsSchemaFilter schemaFilter = new();
        schemaFilter.Apply(schema, new(type, schemaGenerator, schemaRepository));

        schema.Type.Should().NotBe("string");
    }
}
