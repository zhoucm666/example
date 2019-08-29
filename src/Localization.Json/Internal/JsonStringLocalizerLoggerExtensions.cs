using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Localization.Json.Internal
{
    internal static class JsonStringLocalizerLoggerExtensions
    {
        private static readonly Action<ILogger, string, string, CultureInfo, Exception> _searchedLocation;
        static JsonStringLocalizerLoggerExtensions()
        {
            _searchedLocation = LoggerMessage.Define<string, string, CultureInfo>(
                LogLevel.Debug, 
                1, 
                $"{nameof(JsonStringLocalizer)} searched for '{{key}}' in '{{LocationSearched}}' with '{{Culture}}.");
        }

        public static void SearchedLocation(this ILogger logger,string key,string searchedLocation,CultureInfo culture)
        {
            _searchedLocation(logger, key, searchedLocation, culture, null);
        }
    }
}
