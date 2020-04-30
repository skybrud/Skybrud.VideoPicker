using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Social.Google.YouTube.Models.Videos;
using Skybrud.VideoPicker.Models.Videos;

namespace Skybrud.VideoPicker.Providers.YouTube {
    
    public class YouTubeVideoDetails : IVideoDetails {

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

        [JsonProperty("thumbnails")]
        public VideoThumbnail[] Thumbnails { get; }

        #endregion

        #region Constructors

        public YouTubeVideoDetails(YouTubeVideo video) {


            List<VideoThumbnail> thumbnails = new List<VideoThumbnail>();

            if (video.Snippet.Thumbnails.HasDefault) {
                thumbnails.Add(ICanHasThumbnail(video.Snippet.Thumbnails.Default));
            }

            if (video.Snippet.Thumbnails.HasMedium) {
                thumbnails.Add(ICanHasThumbnail(video.Snippet.Thumbnails.Medium));
            }

            if (video.Snippet.Thumbnails.HasHigh) {
                thumbnails.Add(ICanHasThumbnail(video.Snippet.Thumbnails.High));
            }

            if (video.Snippet.Thumbnails.HasStandard) {
                thumbnails.Add(ICanHasThumbnail(video.Snippet.Thumbnails.Standard));
            }

            if (video.Snippet.Thumbnails.HasMaxRes) {
                thumbnails.Add(ICanHasThumbnail(video.Snippet.Thumbnails.MaxRes));
            }


            Id = video.Id;
            Title = video.Snippet.Title;
            Description = video.Snippet.Description;
            Duration = video.ContentDetails.Duration.Value;
            Thumbnails = thumbnails.ToArray();

        }

        private VideoThumbnail ICanHasThumbnail(YouTubeVideoThumbnail thumb) {
            return new VideoThumbnail(thumb.Width, thumb.Height, thumb.Url);
        }

        public YouTubeVideoDetails(JObject obj) {
            Id = obj.GetString("id");
            Title = obj.GetString("title");
            Description = obj.GetString("description");
            Duration = obj.GetDouble("duration", TimeSpan.FromSeconds);
            Thumbnails = obj.GetArrayItems("thumbnails", VideoThumbnail.Parse);
        }

        public static YouTubeVideoDetails Parse(JObject obj) {
            return obj == null ? null : new YouTubeVideoDetails(obj);
        }

        #endregion

    }

}