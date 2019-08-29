using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SwaggerWithVersionAndYaml
{
    public class SwaggerYamlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TemplateMatcher _templateMatcher;
        private const string YamlRouteTemplate = "openapi/{documentName}/swagger_oas{v}_{version}.yaml";

        public SwaggerYamlMiddleware(RequestDelegate next)
        {
            _next = next;
            _templateMatcher = new TemplateMatcher(TemplateParser.Parse(YamlRouteTemplate), new RouteValueDictionary());
        }

        public async Task Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
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
            var isOasv2 = httpContext.Request.Path.Value.Contains("oas2");
            await RespondWithSwaggerYaml(httpContext.Response, swagger,isOasv2);
        }

        private async Task RespondWithSwaggerYaml(HttpResponse response, OpenApiDocument swagger,bool isOas2)
        {
            response.StatusCode = 200;
            response.ContentType = "application/yaml;charset=utf-8";

            var result = isOas2? swagger.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml): swagger.Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml);
            await response.WriteAsync(result, new UTF8Encoding(false));

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
