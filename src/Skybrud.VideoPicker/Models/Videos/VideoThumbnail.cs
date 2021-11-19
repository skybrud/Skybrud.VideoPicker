using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Models.Videos {

    public class VideoThumbnail {

        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Width { get; }

        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Height { get; }

        [JsonProperty("url")]
        public string Url { get; }

        public VideoThumbnail(int width, int height, string url) {
            Width = width;
            Height = height;
            Url = url;
        }

        public VideoThumbnail(JObject obj) {
            Width = obj.GetInt32("width");
            Height = obj.GetInt32("height");
            Url = obj.GetString("url");
        }

        public static VideoThumbnail Parse(JObject obj) {
            return obj == null ? null : new VideoThumbnail(obj);
        }

    }

}