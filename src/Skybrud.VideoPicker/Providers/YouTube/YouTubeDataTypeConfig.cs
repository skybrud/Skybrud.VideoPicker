using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings;
using Skybrud.VideoPicker.PropertyEditors;

namespace Skybrud.VideoPicker.Providers.YouTube {

    public class YouTubeDataTypeConfig : IProviderDataTypeConfig {

        [JsonProperty("enabled")]
        public bool IsEnabled { get; }

        [JsonProperty("consent")]
        public DataTypeConfigOption<bool> RequireConsent { get; }

        [JsonProperty("nocookie")]
        public DataTypeConfigOption<bool> NoCookie { get; }

        [JsonProperty("controls")]
        public DataTypeConfigOption<bool> ShowControls { get; }

        [JsonProperty("autoplay")]
        public DataTypeConfigOption<bool> Autoplay { get; }

        [JsonProperty("loop")]
        public DataTypeConfigOption<bool> Loop { get; }

        public YouTubeDataTypeConfig() : this(null) { }

        public YouTubeDataTypeConfig(JObject value) {

            IsEnabled = value.GetBoolean("enabled");
            
            JToken controls = value?.SelectToken("controls.value");

            RequireConsent = new DataTypeConfigOption<bool>(value.GetBoolean("consent.value"));
            NoCookie = new DataTypeConfigOption<bool>(value.GetBoolean("nocookie.value"));
            ShowControls = new DataTypeConfigOption<bool>(StringUtils.ParseBoolean(controls, true));
            Autoplay = new DataTypeConfigOption<bool>(value.GetBoolean("autoplay.value"));
            Loop = new DataTypeConfigOption<bool>(value.GetBoolean("loop.value"));

        }

    }

}