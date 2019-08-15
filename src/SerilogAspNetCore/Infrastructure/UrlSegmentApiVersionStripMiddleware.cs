using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SerilogAspNetCore.Infrastructure
{
    public class UrlSegmentApiVersionStripMiddleware
    {
        private static readonly string DEFAULT_API_VERSION_PREFIX = "v";

        private readonly string _apiVersionVersion;
        private readonly RequestDelegate _next;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">Next request delegate</param>
        /// <param name="apiVersionPrefix">API version prefix (optional)</param>
        public UrlSegmentApiVersionStripMiddleware(RequestDelegate next, string apiVersionPrefix = null)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _apiVersionVersion = Regex.Escape(apiVersionPrefix ?? DEFAULT_API_VERSION_PREFIX);
            _next = next;
        }

        public Task InvokeAsync(HttpContext httpContext)
        {
            // https://github.com/Microsoft/aspnet-api-versioning/wiki/Version-Format
            // [Version Group.]<Major>.<Minor>[-Status]
            // <Version Group>[.<Major>[.Minor]][-Status]
            var urlSegmentApiVersionRegexes = new Regex[] {
            new Regex($@"{_apiVersionVersion}(?<apiVersion>(?:(?<group>\d{4}-\d{2}-\d{2})\.)?(?<major>\d+)(?:\.(?<minor>\d+))?(?:-(?<status>\w+))?)/?$"),
            new Regex($@"{_apiVersionVersion}(?<apiVersion>(?<group>\d{4}-\d{2}-\d{2})(?:(?:\.(?<major>\d+)(?:\.(?<minor>\d+))))?(?:-(?<status>\w+))?)  /?$"),
            };
            //var uri = new Uri(httpContext.Request.Path.ToUriComponent());
            var uri = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path.ToUriComponent()}");
            var apiVersionSegment = uri.Segments
                .Where(segment => urlSegmentApiVersionRegexes.Any(regex => regex.Match(segment).Success))
                .FirstOrDefault();
            if (apiVersionSegment != null)
            {
                var newPath = string.Join("", uri.Segments.Where(segment => segment != apiVersionSegment));
                httpContext.Request.Path = new PathString(newPath);
                var match = urlSegmentApiVersionRegexes.Select(regex => regex.Match(apiVersionSegment))
                    .Where(regex => regex.Success)
                    .First();
                var rawApiVersion = match.Groups["apiVersion"].Value;
                var feature = httpContext.Features.Get<IApiVersioningFeature>();
                feature.RawRequestedApiVersion = rawApiVersion;
            }
            return _next(httpContext);
        }
    }
}
