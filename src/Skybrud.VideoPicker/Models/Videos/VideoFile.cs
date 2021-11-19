using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Models.Videos {
    
    public class VideoFile {
        
        [JsonProperty("width", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Width { get; }

        [JsonProperty("height", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Height { get; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; }

        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long Size { get; }

        public VideoFile(int width, int height, string url, string type, long size) {
            Width = width;
            Height = height;
            Url = url;
            Type = type;
            Size = size;
        }

        public VideoFile(JObject obj) {
            Width = obj.GetInt32("width");
            Height = obj.GetInt32("height");
            Url = obj.GetString("link");
            Type = obj.GetString("type");
            Size = obj.GetInt32("size");
        }

        public static VideoFile Parse(JObject obj) {
            return obj == null ? null : new VideoFile(obj);
        }

    }

}