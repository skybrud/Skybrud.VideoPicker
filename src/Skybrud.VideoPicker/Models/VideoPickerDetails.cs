using System;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Time;

namespace Skybrud.VideoPicker.Models {
    
    public class VideoPickerDetails : JsonObjectBase {

        #region Properties

        /// <summary>
        /// Gets the ID of the video.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets a tiemstamp for when the video was originally published.
        /// </summary>
        public DateTime Published { get; private set; }

        /// <summary>
        /// Gets the original title of the video.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the original description of the video.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the duration of the video.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Gets an array of thumbnails of the video.
        /// </summary>
        public VideoPickerThumbnail[] Thumbnails { get; private set; }

        #endregion

        #region Constructors

        protected VideoPickerDetails(JObject obj) : base(obj) {
            Id = obj.GetString("id");
            Published = obj.GetInt32("published", TimeUtils.GetDateTimeFromUnixTime);
            Title = obj.GetString("title");
            Description = obj.GetString("description");
            Duration = obj.GetDouble("duration", TimeSpan.FromSeconds);
            Thumbnails = obj.GetArrayItems("thumbnails", VideoPickerThumbnail.Parse);
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