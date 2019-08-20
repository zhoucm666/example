using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerWithVersionAndYaml
{
    public class LowercaseDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var originalPaths = swaggerDoc.Paths;
            var newPaths = new Dictionary<string, PathItem>();
            var removeKeys = new List<string>();
            foreach(var path in originalPaths)
            {
                var newKey = LowercaseEverythingButParameters(path.Key);
                if (newKey != path.Key)
                {
                    removeKeys.Add(path.Key);
                    newPaths.Add(newKey, path.Value);
                }
            }

            foreach (var path in newPaths)
                swaggerDoc.Paths.Add(path.Key, path.Value);

            //	remove the old keys
            foreach (var key in removeKeys)
                swaggerDoc.Paths.Remove(key);
        }

        private static string LowercaseEverythingButParameters(string key)
        {
            return string.Join('/', key.Split('/')
                .Select(x => x.Contains("{")
                    ? x
                    : x.ToLower()));
        }
    }

    public class LowercaseDocumentFilterV2 : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths = swaggerDoc.Paths.ToDictionary(entry => LowercaseEverythingButParameters(entry.Key), entry => entry.Value);
        }

        private static string LowercaseEverythingButParameters(string key)
        {
            return string.Join('/', key.Split('/').Select(x => x.Contains("{") ? x : x.ToLower()));
        }
    }
}
