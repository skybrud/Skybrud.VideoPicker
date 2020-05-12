using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings;
using Skybrud.VideoPicker.PropertyEditors;

namespace Skybrud.VideoPicker.Providers.Vimeo {

    public class VimeoDataTypeConfig : IProviderDataTypeConfig {

        [JsonProperty("enabled")]
        public bool IsEnabled { get; }

        [JsonProperty("consent")]
        public DataTypeConfigOption<bool> RequireConsent { get; }

        [JsonProperty("autoplay")]
        public DataTypeConfigOption<bool> Autoplay { get; }

        [JsonProperty("loop")]
        public DataTypeConfigOption<bool> Loop { get; }

        [JsonProperty("color")]
        public DataTypeConfigOption<string> Color { get; }

        [JsonProperty("title")]
        public DataTypeConfigOption<bool> ShowTitle { get; }

        [JsonProperty("byline")]
        public DataTypeConfigOption<bool> ShowByline { get; }

        [JsonProperty("portrait")]
        public DataTypeConfigOption<bool> ShowPortrait { get; }

        public VimeoDataTypeConfig() : this(null) { }

        public VimeoDataTypeConfig(JObject value) {

            IsEnabled = value.GetBoolean("enabled");
            
            RequireConsent = new DataTypeConfigOption<bool>(value.GetBoolean("consent.value"));
            Autoplay = new DataTypeConfigOption<bool>(value.GetBoolean("autoplay.value"));
            Loop = new DataTypeConfigOption<bool>(value.GetBoolean("loop.value"));
            Color = new DataTypeConfigOption<string>(value.GetString("color.value"));
            ShowTitle = new DataTypeConfigOption<bool>(StringUtils.ParseBoolean(value?.SelectToken("title.value"), true));
            ShowByline = new DataTypeConfigOption<bool>(StringUtils.ParseBoolean(value?.SelectToken("byline.value"), true));
            ShowPortrait = new DataTypeConfigOption<bool>(StringUtils.ParseBoolean(value?.SelectToken("portrait.value"), true));

        }

    }

}