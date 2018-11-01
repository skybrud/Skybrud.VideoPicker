using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Models {
    
    public class VideoPickerItem : JsonObjectBase {

        private VideoPickerImage _thumbnail;

        #region Properties

        [JsonProperty("url")]
        public string Url { get; internal set; }

        [JsonProperty("type")]
        public string Type { get; internal set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; internal set; }

        [JsonIgnore]
        public bool HasTitle => !String.IsNullOrWhiteSpace(Title);

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; internal set; }

        [JsonIgnore]
        public bool HasDescription => !String.IsNullOrWhiteSpace(Description);

        [JsonProperty("details")]
        public VideoPickerDetails Details { get; internal set; }

        /// <summary>
        /// Gets the media ID of the selected thumbnail, or <c>0</c> if no thumbnail has been selected.
        /// </summary>
        [JsonProperty("thumbnailId")]
        public int ThumbnailId { get; }

        /// <summary>
        /// Gets a reference to the editorial thumbnail of the image, or <c>null</c> if not selected.
        /// </summary>
        [JsonProperty("thumbnail")]
        public VideoPickerImage Thumbnail => _thumbnail == null && ThumbnailId > 0 ? _thumbnail = VideoPickerImage.GetFromId(ThumbnailId) : _thumbnail;

        [JsonIgnore]
        public bool HasThumbnail => Thumbnail != null;

        [JsonIgnore]
        public string ThumbnailCropUrl => Thumbnail?.CropUrl;

        /// <summary>
        /// Gets whether the item is valid.
        /// </summary>
        [JsonIgnore]
        public bool IsValid => Details != null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an empty video picker item.
        /// </summary>
        internal VideoPickerItem() : base(null) {
            Url = "";
            Type = "";
            Title = "";
            Description = "";
        }

        protected VideoPickerItem(JObject obj) : base(obj) {
            Url = obj.GetString("url");
            Type = obj.GetString("type");
            Title = obj.GetString("title");
            Description = obj.GetString("description");
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