using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace SwaggerWithVersionAndYaml
{
    public class LowercaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var openApiPathItems = swaggerDoc.Paths.ToDictionary(item => ToCamelcase(item.Key), item => item.Value);
            swaggerDoc.Paths.Clear();
            openApiPathItems.ToList().ForEach(item => swaggerDoc.Paths.Add(item.Key, item.Value));
        }

        private string ToCamelcase(string value)
        {
            return string.Join('/', value.Split('/').Select(x => x.Contains('{') || x.Length < 2 ? x : char.ToLowerInvariant(x[0]) + x.Substring(1)));
        }

    }
}
