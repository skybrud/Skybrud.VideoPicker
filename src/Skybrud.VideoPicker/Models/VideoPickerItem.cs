using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Models {
    
    public class VideoPickerItem : JsonObjectBase {

        private VideoPickerImage _thumbnail;

        #region Properties

        [JsonProperty("url")]
        public string Url { get; private set; }

        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("details")]
        public VideoPickerDetails Details { get; private set; }

        /// <summary>
        /// Gets the media ID of the selected thumbnail, or <code>0</code> if no thumbnail has been selected.
        /// </summary>
        [JsonProperty("thumbnailId")]
        public int ThumbnailId { get; private set; }

        /// <summary>
        /// Gets a reference to the editorial thumbnail of the image, or <code>null</code> if not selected.
        /// </summary>
        [JsonProperty("thumbnail")]
        public VideoPickerImage Thumbnail {
            get {
                return (_thumbnail == null && ThumbnailId > 0 ? _thumbnail = VideoPickerImage.GetFromId(ThumbnailId) : _thumbnail);
            }
        }

        public bool HasThumbnail {
            get { return Thumbnail != null; }
        }

        public string ThumbnailCropUrl {
            get { return Thumbnail == null ? null : Thumbnail.CropUrl; }
        }

        /// <summary>
        /// Gets whether the item is valid.
        /// </summary>
        public bool IsValid {
            get { return Details != null; }
        }

        #endregion

        #region Constructors

        protected VideoPickerItem(JObject obj) : base(obj) {
            Url = obj.GetString("url");
            Type = obj.GetString("type");
            Details = obj.GetObject("details", VideoPickerDetails.Parse);
            ThumbnailId = obj.GetInt32("thumbnailId");
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets an instance of <see cref="VideoPickerItem"/> from the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        /// <returns>An instance of <see cref="VideoPickerItem"/>.</returns>
        public static VideoPickerItem Parse(JObject obj) {
            return obj == null ? null : new VideoPickerItem(obj );
        }

        #endregion

    }

}