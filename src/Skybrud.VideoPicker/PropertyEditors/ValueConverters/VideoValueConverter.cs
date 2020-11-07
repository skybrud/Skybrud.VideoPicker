using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Providers;
using Skybrud.VideoPicker.Services;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.VideoPicker.PropertyEditors.ValueConverters {

    public class VideoValueConverter : PropertyValueConverterBase {

        private readonly VideoPickerService _videoPickerService;

        #region Constructors

        public VideoValueConverter() {
            _videoPickerService = new VideoPickerService();
        }

        #endregion

        public override bool IsConverter(IPublishedPropertyType propertyType) {
            return propertyType.EditorAlias == "Skybrud.VideoPicker.Video";
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview) {

            string str = source?.ToString();
            if (string.IsNullOrWhiteSpace(str)) return null;

            JObject obj = JsonUtils.ParseJsonObject(str);

            string providerAlias = obj.GetString("provider.alias");

            IVideoProvider provider = _videoPickerService.Providers.FirstOrDefault(x => x.Alias == providerAlias);
            if (provider == null) return null;

            VideoConfiguration config = propertyType.DataType.Configuration as VideoConfiguration;

            IProviderDataTypeConfig providerConfig = null;

            config?.Providers?.TryGet(provider, out providerConfig);

            return provider?.ParseValue(obj, providerConfig);

        }
        
        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return inter;
        }

        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return null;
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
            return PropertyCacheLevel.Snapshot;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
            return typeof(VideoPickerValue);
        }

    }

}