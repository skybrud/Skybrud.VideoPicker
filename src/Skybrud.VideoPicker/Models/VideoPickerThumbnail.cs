using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.Google.YouTube.Models.Videos;
using Skybrud.Social.TwentyThree.Models.Photos;

namespace Skybrud.VideoPicker.Models {
    
    public class VideoPickerThumbnail : JsonObjectBase {

        #region Properties

        /// <summary>
        /// Gets the alias of the thumbnail format/size. The alias may not be available - eg. for Vimeo videos.
        /// </summary>
        [JsonProperty("alias", NullValueHandling = NullValueHandling.Ignore)]
        public string Alias { get; internal set; }

        /// <summary>
        /// Gets the URL of the thumbnail image.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; internal set; }

        /// <summary>
        /// Gets the width of the thumbnail.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; internal set; }

        /// <summary>
        /// Gets the height of the thumbnail.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; internal set; }

        #endregion

        #region Constructors

        internal VideoPickerThumbnail() : base(null) { }

        internal VideoPickerThumbnail(string schemeAndHost, TwentyThreeThumbnail thumbnail) : base(null) {
            Alias = thumbnail.Alias;
            Url = schemeAndHost + thumbnail.Url;
            Width = thumbnail.Width;
            Height = thumbnail.Height;
        }

        internal VideoPickerThumbnail(string alias, YouTubeVideoThumbnail thumbnail) : base(null) {
            Alias = alias;
            Url = thumbnail.Url;
            Width = thumbnail.Width;
            Height = thumbnail.Height;
        }

        protected VideoPickerThumbnail(JObject obj) : base(obj) {
            Alias = obj.GetString("alias");
            Url = obj.GetString("url");
            Width = obj.GetInt32("width");
            Height = obj.GetInt32("height");
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets an instance of <see cref="VideoPickerThumbnail"/> from the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        /// <returns>An instance of <see cref="VideoPickerThumbnail"/>.</returns>
        public static VideoPickerThumbnail Parse(JObject obj) {
            return obj == null ? null : new VideoPickerThumbnail(obj);
        }

        /// <summary>
        /// Gets a new <see cref="VideoPickerThumbnail"/> from the specified Vimeo <paramref name="thumbnail"/>.
        /// </summary>
        /// <param name="thumbnail">An instance of <see cref="JObject"/> representing the Vimeo thumbnail.</param>
        /// <returns></returns>
        internal static VideoPickerThumbnail GetFromVimeo(JObject thumbnail) {
            return new VideoPickerThumbnail {
                Url = thumbnail.GetString("link"),
                Width = thumbnail.GetInt32("width"),
                Height = thumbnail.GetInt32("height")
            };
        }

        #endregion

    }

}