using Skybrud.VideoPicker.Models.Options;

namespace Skybrud.VideoPicker.Providers.Vimeo {
    
    public class VimeoVideoOptions : IVideoOptions {

        public string VideoId { get; }

        public VimeoVideoOptions(string videoId) {
            VideoId = videoId;
        }

    }

}