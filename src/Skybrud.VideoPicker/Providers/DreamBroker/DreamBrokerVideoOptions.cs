using Skybrud.VideoPicker.Models.Options;

namespace Skybrud.VideoPicker.Providers.DreamBroker {

    public class DreamBrokerVideoOptions : IVideoOptions {

        public string ChannelId { get; }

        public string VideoId { get; }

        public DreamBrokerVideoOptions(string channelId, string videoId) {
            ChannelId = channelId;
            VideoId = videoId;
        }

    }

}