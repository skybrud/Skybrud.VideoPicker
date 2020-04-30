using Newtonsoft.Json;
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
        public string Embed { get; }

        #endregion

        #region Constructors

        public VideoPickerValue(IVideoProviderDetails provider, IVideoDetails details, string embed) {
            Provider = provider;
            Details = details;
            Embed = embed;
        }

        public VideoPickerValue(IVideoProviderDetails provider, IVideoDetails details) {
            Provider = provider;
            Details = details;
        }

        #endregion

    }

}