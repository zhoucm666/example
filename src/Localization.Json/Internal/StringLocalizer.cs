using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace Localization.Json.Internal
{
    internal class StringLocalizer:IStringLocalizer
    {
        private IStringLocalizer _localizer;
        public StringLocalizer(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(string.Empty, PathHelpers.GetApplicationRoot());
        }

        public LocalizedString this[string name] => _localizer[name];

        public LocalizedString this[string name, params object[] arguments] => _localizer[name,arguments];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return _localizer.WithCulture(culture);
        }
    }
}
