using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.PropertyEditors.Video {
    
    [JsonConverter(typeof(Hest))]
    public class DefaultProviderDataTypeConfig : IProviderDataTypeConfig {

        [JsonProperty("enabled")]
        public bool IsEnabled { get; }

        [JsonIgnore]
        public JObject Value { get; }

        public DefaultProviderDataTypeConfig(JObject value) {
            Value = value;
            IsEnabled = value.GetBoolean("enabled");
        }

    }

}