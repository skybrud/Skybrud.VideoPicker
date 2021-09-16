using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Time;

namespace Skybrud.VideoPicker.Models {
    
    public class VideoPickerDetails : JsonObjectBase {

        #region Properties

        /// <summary>
        /// Gets the ID of the video.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the URL for accessing a page with a player for the video.
        /// </summary>
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; internal set; }

        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain { get; internal set; }

        /// <summary>
        /// Gets the type of the video. Eg. <c>video</c> or <c>spot</c>.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; internal set; }

        /// <summary>
        /// Gets a tiemstamp for when the video was originally published.
        /// </summary>
        [JsonProperty("published", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(TimeConverter), TimeFormat.UnixTime)]
        public EssentialsTime Published { get; internal set; }

        /// <summary>
        /// Gets the original title of the video.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; internal set; }

        /// <summary>
        /// Gets the original description of the video.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; internal set; }

        /// <summary>
        /// Gets the duration of the video.
        /// </summary>
        [JsonProperty("duration", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// Gets an array of thumbnails of the video.
        /// </summary>
        [JsonProperty("thumbnails", NullValueHandling = NullValueHandling.Ignore)]
        public VideoPickerThumbnail[] Thumbnails { get; internal set; }

        /// <summary>
        /// Gets an array of formats of the video.
        /// </summary>
        [JsonProperty("formats", NullValueHandling = NullValueHandling.Ignore)]
        public VideoPickerFormat[] Formats { get; internal set; }

        /// <summary>
        /// Gets an embed code as returned by the provider.
        /// </summary>
        [JsonProperty("embed", NullValueHandling = NullValueHandling.Ignore)]
        public string Embed { get; internal set; }

        #endregion

        #region Constructors

        internal VideoPickerDetails() : base(null) {
            Type = "video";
        }

        protected VideoPickerDetails(JObject obj) : base(obj) {
            Id = obj.GetString("id");
            Domain = obj.GetString("domain");
            Type = obj.GetString("type") ?? "video";
            Published = obj.GetInt32("published", EssentialsTime.FromUnixTimestamp);
            Title = obj.GetString("title");
            Description = obj.GetString("description");
            Duration = obj.GetDouble("duration", TimeSpan.FromSeconds);
            Thumbnails = obj.GetArrayItems("thumbnails", VideoPickerThumbnail.Parse);
            Formats = obj.GetArrayItems("formats", VideoPickerFormat.Parse);
            Embed = obj.GetString("embed");
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets an instance of <see cref="VideoPickerDetails"/> from the specified <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        /// <returns>An instance of <see cref="VideoPickerDetails"/>.</returns>
        public static VideoPickerDetails Parse(JObject obj) {
            return obj == null ? null : new VideoPickerDetails(obj );
        }

        #endregion

    }

}