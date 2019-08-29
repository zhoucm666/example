using Microsoft.Extensions.Localization;

namespace Localization.Json
{
    public class JsonLocalizationOptions:LocalizationOptions
    {
        public ResourcesType ResourcesType { get; set; } = ResourcesType.TypeBased;
    }
}
