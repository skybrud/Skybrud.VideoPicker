using Skybrud.VideoPicker.Models.Options;

namespace Skybrud.VideoPicker.Providers.Vimeo {
    
    public class VimeoVideoOptions : IVideoOptions {

        public long VideoId { get; }

        public VimeoVideoOptions(long videoId) {
            VideoId = videoId;
        }

    }

}