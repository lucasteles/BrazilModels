using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BrazilModels.Swagger;

/// <summary>
/// Defines schema for CPF/CNPJ on Swagger
/// </summary>
public sealed class BrazilModelsSchemaFilter : ISchemaFilter
{
    static readonly HashSet<Type> validTypes = new(new[]
    {
        typeof(Cnpj), typeof(Cpf), typeof(CpfCnpj), typeof(Email), typeof(PhoneNumber),
    });

    /// <inheritdoc />
    public void Apply(
        OpenApiSchema schema,
        SchemaFilterContext context
    )
    {
        if (!validTypes.Contains(context.Type))
            return;

        OpenApiSchema idSchema = new()
        {
            Type = "string",
            Format = "",
        };
        schema.Type = idSchema.Type;
        schema.Format = idSchema.Format;
        schema.Example = idSchema.Example;
        schema.Default = idSchema.Default;
        schema.Properties = idSchema.Properties;
        schema.Nullable = false;
    }
}
