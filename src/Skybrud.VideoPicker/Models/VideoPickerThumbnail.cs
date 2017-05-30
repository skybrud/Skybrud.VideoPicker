using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Models {
    
    public class VideoPickerThumbnail : JsonObjectBase {

        #region Properties

        /// <summary>
        /// Gets the alias of the thumbnail format/size. The alias is only available for YouTube videos.
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the URL of the thumbnail image.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the width of the thumbnail.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the thumbnail.
        /// </summary>
        public int Height { get; private set; }

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