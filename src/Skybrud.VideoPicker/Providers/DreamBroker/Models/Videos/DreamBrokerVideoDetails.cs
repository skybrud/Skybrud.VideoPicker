using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.VideoPicker.Models.Videos;

namespace Skybrud.VideoPicker.Providers.DreamBroker.Models.Videos {

    public class DreamBrokerVideoDetails : IVideoDetails {

        #region Properties

        [JsonProperty("id")]
        public string Id { get; }

        [JsonProperty("channelId")]
        public string ChannelId { get; }

        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("thumbnails")]
        public VideoThumbnail[] Thumbnails { get; }

        /// <summary>
        /// Gets the HTML embed code as received from the Dream Broker OEmbed endpoint.
        /// </summary>
        [JsonProperty("embed")]
        public string Embed { get; }

        #endregion

        #region Constructors

        public DreamBrokerVideoDetails(string videoId, string channelId, DreamBrokerOEmbed oembed) {
            Id = videoId;
            ChannelId = channelId;
            Title = oembed.Title;
            Thumbnails = new List<VideoThumbnail> {
                new VideoThumbnail(oembed.ThumbnailWidth, oembed.ThumbnailHeight, oembed.ThumbnailUrl),
                new VideoThumbnail(1020, 576, $"https://dreambroker.com/channel/{channelId}/{videoId}/get/poster")
            }.ToArray();
            Embed = oembed.Html;
        }

        public DreamBrokerVideoDetails(JObject obj) {
            Id = obj.GetString("id");
            ChannelId = obj.GetString("channelId");
            Title = obj.GetString("title");
            Thumbnails = obj.GetArrayItems("thumbnails", VideoThumbnail.Parse);
            Embed = obj.GetString("embed");
        }

        public static DreamBrokerVideoDetails Parse(JObject obj) {
            return obj == null ? null : new DreamBrokerVideoDetails(obj);
        }

        #endregion

    }

}