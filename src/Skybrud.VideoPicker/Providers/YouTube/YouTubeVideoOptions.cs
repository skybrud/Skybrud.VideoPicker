using Skybrud.VideoPicker.Models.Options;

namespace Skybrud.VideoPicker.Providers.YouTube {
    
    public class YouTubeVideoOptions : IVideoOptions {

        public string VideoId { get; }

        public YouTubeVideoOptions(string videoId) {
            VideoId = videoId;
        }

    }

}