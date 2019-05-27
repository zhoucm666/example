using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;

namespace SwaggerWithVersionAndYaml
{
    public class AutoRegisterSwagger : IStartupFilter
    {
        private readonly IApiVersionDescriptionProvider _provider;
        public AutoRegisterSwagger(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseStaticFiles();
                app.UseStaticFiles(new StaticFileOptions
                {
                    ServeUnknownFileTypes = true,
                    DefaultContentType = "application/yaml"
                });
                app.UseSwagger();
                app.UseMiddleware<SwaggerYamlMiddleware>();
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in _provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant()
                            );
                    }
                });
                next(app);
            };
        }
    }
}
