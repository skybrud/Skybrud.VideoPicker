using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.Vimeo.Models.Videos;
using Skybrud.VideoPicker.Models.Videos;

namespace Skybrud.VideoPicker.Providers.Vimeo {
    
    public class VimeoVideoDetails : IVideoDetails {

        #region Properties

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("description")]
        public string Description { get; }
        
        /// <summary>
        /// Gets the duration of the video.
        /// </summary>
        [JsonProperty("duration")]
        [JsonConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan Duration { get; }

        [JsonProperty("width")]
        public int Width { get; }

        [JsonProperty("height")]
        public int Height { get; }

        [JsonProperty("thumbnails")]
        public VideoThumbnail[] Thumbnails { get; }

        [JsonProperty("files")]
        public VideoFile[] Files { get; }

        #endregion

        #region Constructors

        public VimeoVideoDetails(VimeoVideo video) {

            List<VideoThumbnail> thumbnails = new List<VideoThumbnail>();

            foreach (VimeoVideoPicture size in video.Pictures.Sizes) {
                thumbnails.Add(new VideoThumbnail(size.Width, size.Height, size.Link));
            }

            List<VideoFile> files = new List<VideoFile>();

            foreach(VimeoVideoFile file in video.Files) {
                files.Add(new VideoFile(file.Width, file.Height, file.Link, file.Type, file.Size));
            }

            Id = video.Id.ToString();
            Title = video.Name;
            Description = video.Description;
            Duration = video.Duration;
            Width = video.Width;
            Height = video.Height;
            Thumbnails = thumbnails.ToArray();
            Files = files.ToArray();

        }

        public VimeoVideoDetails(JObject obj) {
            Id = obj.GetString("id");
            Title = obj.GetString("title");
            Description = obj.GetString("description");
            Duration = obj.GetDouble("duration", TimeSpan.FromSeconds);
            Width = obj.GetInt32("width");
            Height = obj.GetInt32("height");
            Thumbnails = obj.GetArrayItems("thumbnails", VideoThumbnail.Parse);
            Files = obj.GetArrayItems("files", VideoFile.Parse);
        }

        #endregion

        #region Static methods

        public static VimeoVideoDetails Parse(JObject obj) {
            return obj == null ? null : new VimeoVideoDetails(obj);
        }

        #endregion

    }

}