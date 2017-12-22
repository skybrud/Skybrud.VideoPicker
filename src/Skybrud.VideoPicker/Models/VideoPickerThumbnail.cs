using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Models {
    
    public class VideoPickerThumbnail : JsonObjectBase {

        #region Properties

        /// <summary>
        /// Gets the alias of the thumbnail format/size. The alias is only available for YouTube videos.
        /// </summary>
        [JsonProperty("alias", NullValueHandling = NullValueHandling.Ignore)]
        public string Alias { get; }

        /// <summary>
        /// Gets the URL of the thumbnail image.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; }

        /// <summary>
        /// Gets the width of the thumbnail.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; }

        /// <summary>
        /// Gets the height of the thumbnail.
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; }

        #endregion

        #region Constructors

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

        #endregion

    }

}