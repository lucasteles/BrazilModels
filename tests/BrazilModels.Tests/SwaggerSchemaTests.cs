using System.Text.Json;
using BrazilModels;
using BrazilModels.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BrazilModels.Tests;

public class SwaggerSchemaTests : BaseTest
{
    [TestCase(typeof(Cpf))]
    [TestCase(typeof(Cnpj))]
    public void CanShowImplementationTypeExampleWithSwaggerSchemaFilter(Type type)
    {
        var schemaGenerator = new SchemaGenerator(new(),
            new JsonSerializerDataContractResolver(new()));
        var provider = new ServiceCollection().BuildServiceProvider();
        var schemaRepository = new SchemaRepository();

        var schema = schemaGenerator.GenerateSchema(type, schemaRepository);
        var schemaFilter = new AnnotationsSchemaFilter(provider);
        schemaFilter.Apply(schema, new SchemaFilterContext(type, schemaGenerator, schemaRepository));

        schema.Type.Should().Be("string");
        schema.Format.Should().Be(string.Empty);
        schema.Example.Should().BeNull();
    }
}
