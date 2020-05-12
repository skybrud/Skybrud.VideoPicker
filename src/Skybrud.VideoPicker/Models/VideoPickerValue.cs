using System.Web;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters;
using Skybrud.VideoPicker.Models.Providers;
using Skybrud.VideoPicker.Models.Videos;

namespace Skybrud.VideoPicker.Models {

    public class VideoPickerValue {

        #region Properties

        [JsonProperty("provider")]
        public IVideoProviderDetails Provider { get; }

        [JsonProperty("details")]
        public IVideoDetails Details { get; }

        [JsonProperty("embed", NullValueHandling = NullValueHandling.Ignore)]
        public IVideoEmbedOptions Embed { get; }

        [JsonProperty("html", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ToStringJsonConverter))]
        public IHtmlString Html => Embed?.GetHtml();

        #endregion

        #region Constructors

        public VideoPickerValue(IVideoProviderDetails provider, IVideoDetails details, IVideoEmbedOptions embed) {
            Provider = provider;
            Details = details;
            Embed = embed;
        }

        #endregion

    }

}