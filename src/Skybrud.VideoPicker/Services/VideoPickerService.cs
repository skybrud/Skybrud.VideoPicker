using System.Collections.Generic;
using Skybrud.VideoPicker.Models;
using Skybrud.VideoPicker.Models.Config;
using Skybrud.VideoPicker.Models.Options;
using Skybrud.VideoPicker.Providers;
using Skybrud.VideoPicker.Providers.DreamBroker;
using Skybrud.VideoPicker.Providers.YouTube;

namespace Skybrud.VideoPicker.Services {
    
    public class VideoPickerService {

        public List<IVideoProvider> Providers { get; }

        public VideoPickerService() {
        
            Providers = new List<IVideoProvider> {
                new DreamBrokerVideoProvider(),
                new YouTubeVideoProvider()
            };

        }

        public virtual VideoPickerValue GetVideo(VideoPickerConfig config, string source) {

            foreach (IVideoProvider provider in Providers) {
                if (provider.IsMatch(config, source, out IVideoOptions options)) {
                    return provider.GetVideo(config, options);
                }
            }

            return null;

        }

    }

}