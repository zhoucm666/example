using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SwaggerWithVersionAndYaml
{
    public class SwaggerYamlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TemplateMatcher _templateMatcher;
        private const string YamlRouteTemplate = "swagger/{documentName}/swagger_{version}.yaml";

        public SwaggerYamlMiddleware(RequestDelegate next)
        {
            _next = next;
            _templateMatcher = new TemplateMatcher(TemplateParser.Parse(YamlRouteTemplate), new RouteValueDictionary());
        }

        public async Task Invoke(HttpContext httpContext,ISwaggerProvider swaggerProvider)
        {
            if (!RequestingSwaggerDocument(httpContext.Request, out string documentName))
            {
                await _next(httpContext);
                return;
            }

            var basePath = string.IsNullOrEmpty(httpContext.Request.PathBase)
                ? null
                : httpContext.Request.PathBase.ToString();

            var swagger = swaggerProvider.GetSwagger(documentName, null, basePath);

            await RespondWithSwaggerYaml(httpContext.Response, swagger);
        }

        private async Task RespondWithSwaggerYaml(HttpResponse response, SwaggerDocument swagger)
        {
            response.StatusCode = 200;
            response.ContentType = "application/yaml;charset=utf-8";
            var builder = new SerializerBuilder();
            builder.WithNamingConvention(new CamelCaseNamingConvention());
            builder.WithTypeInspector(innerInspector => new PropertiesIgnoreTypeInspector(innerInspector));
            var serializer = builder.Build();
            StringBuilder yamlBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(yamlBuilder))
            {
                serializer.Serialize(textWriter, swagger);
                var result = yamlBuilder.ToString();
                result = result.Replace("swagger: 2.0", "swagger: \"2.0\"", StringComparison.OrdinalIgnoreCase);
                result = Regex.Replace(result, "\bref:", "$ref:");
                await response.WriteAsync(result, new UTF8Encoding(false));
            }
        }

        private bool RequestingSwaggerDocument(HttpRequest request, out string documentName)
        {
            documentName = null;
            if (request.Method != "GET") return false;

            var routeValues = new RouteValueDictionary();
            if (!_templateMatcher.TryMatch(request.Path, routeValues) || !routeValues.ContainsKey("documentName")) return false;

            documentName = routeValues["documentName"].ToString();
            return true;
        }
    }
}
