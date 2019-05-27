using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SwaggerWithVersionAndYaml;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerWithVersionAndYaml(this IServiceCollection services)
        {
            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();
                options.DocumentFilter<YamlDocumentFilter>();
            });
            services.AddTransient<IStartupFilter, AutoRegisterSwagger>();
        }
    }
}
