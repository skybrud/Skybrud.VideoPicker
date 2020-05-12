using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.VideoPicker.Providers.DreamBroker {

    public class DreamBrokerOEmbed {

        public string Title { get; }

        public string Html { get; }

        public string ThumbnailUrl { get; }

        public int ThumbnailWidth { get; }

        public int ThumbnailHeight { get; }

        public DreamBrokerOEmbed(JObject obj) {
            Title = obj.GetString("title");
            Html = obj.GetString("html");
            ThumbnailUrl = obj.GetString("thumbnail_url");
            ThumbnailWidth = obj.GetInt32("thumbnail_width");
            ThumbnailHeight = obj.GetInt32("thumbnail_height");
        }

    }

}