using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Models.Videos
{
    public class VideoFile
    {
        [JsonProperty("width")]
        public int Width { get; }

        [JsonProperty("height")]
        public int Height { get; }

        [JsonProperty("url")]
        public string Url { get; }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("size")]
        public int Size { get; }

        public VideoFile(int width, int height, string url, string type, int size)
        {
            Width = width;
            Height = height;
            Url = url;
            Type = type;
            Size = size;
        }

        public VideoFile(JObject obj)
        {
            Width = obj.GetInt32("Width");
            Height = obj.GetInt32("Height");
            Url = obj.GetString("Link");
            Type = obj.GetString("Type");
            Size = obj.GetInt32("Size");
        }

        public static VideoFile Parse(JObject obj)
        {
            return obj == null ? null : new VideoFile(obj);
        }
    }
}
