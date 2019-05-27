using Microsoft.AspNetCore.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.TypeInspectors;

namespace SwaggerWithVersionAndYaml
{
    public class YamlDocumentFilter : IDocumentFilter
    {
        private readonly IHostingEnvironment hostingEnvironment;
        public YamlDocumentFilter(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            try
            {
                var builder = new SerializerBuilder();
                builder.WithNamingConvention(new CamelCaseNamingConvention());
                builder.WithTypeInspector(innerInspector => new PropertiesIgnoreTypeInspector(innerInspector));
                var serializer = builder.Build();
                using (var writer = new StringWriter())
                {
                    serializer.Serialize(writer, swaggerDoc);
                    var file = Path.Combine(hostingEnvironment.ContentRootPath, "swagger.yaml");
                    using (var stream = new StreamWriter(file))
                    {
                        var result = writer.ToString();
                        stream
                            .WriteLine(result
                                .Replace("2.0", "\"2.0\"", StringComparison.OrdinalIgnoreCase)
                                .Replace("\bref:", "$ref:", StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    
}
