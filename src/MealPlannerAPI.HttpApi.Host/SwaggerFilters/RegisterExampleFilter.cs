using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Volo.Abp.Account;

namespace MealPlannerAPI.SwaggerFilters
{
    public class RegisterExampleFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type != typeof(RegisterDto))
            {
                return;
            }

            if (schema is not OpenApiSchema openApiSchema)
            {
                return;
            }

            openApiSchema.Examples = new List<JsonNode>
            {
                new JsonObject
                {
                    ["userName"] = "string",
                    ["emailAddress"] = "user@example.com",
                    ["password"] = "string",
                    ["appName"] = "MealPlanner",
                    ["extraProperties"] = new JsonObject
                    {
                        ["name"] = "John",
                        ["surname"] = "Doe"
                    }
                }
            };
        }
    }
}