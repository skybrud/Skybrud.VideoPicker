using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.VideoPicker.Providers;
using Skybrud.VideoPicker.Services;

namespace Skybrud.VideoPicker.PropertyEditors.Video {
    
    [JsonConverter(typeof(Hest))]
    public class VideoPickerProvidersConfig : IEnumerable<IProviderDataTypeConfig> {

        [JsonIgnore]
        public JObject Json { get; }

        public ReadOnlyDictionary<string, IProviderDataTypeConfig> Providers { get; }

        public VideoPickerProvidersConfig(VideoPickerService service, JObject obj) {

            Json = obj;

            Dictionary<string, IProviderDataTypeConfig> temp = new Dictionary<string, IProviderDataTypeConfig>();

            foreach (JProperty property in obj.Properties()) {

                if (property.Value is JObject value) {

                    if (service.Providers.TryGet(property.Name, out IVideoProvider provider)) {
                        temp.Add(property.Name, provider.ParseDataTypeConfig(value));
                    } else {
                        temp.Add(property.Name, new DefaultProviderDataTypeConfig(value));
                    }

                }

            }

            Providers = new ReadOnlyDictionary<string, IProviderDataTypeConfig>(temp);

        }

        public bool TryGet(IVideoProvider provider, out IProviderDataTypeConfig config) {
            return Providers.TryGetValue(provider.Alias, out config);
        }

        public bool TryGet(string providerAlias, out IProviderDataTypeConfig config) {
            return Providers.TryGetValue(providerAlias, out config);
        }

        public IEnumerator<IProviderDataTypeConfig> GetEnumerator() {
            return Providers.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

}